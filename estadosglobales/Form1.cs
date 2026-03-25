using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace estadosglobales
{
    public partial class Form1 : Form
    {
        private long currentTick = 0;
        private List<MessageEvent> messages = new List<MessageEvent>();
        private List<NodeEvent> nodeEvents = new List<NodeEvent>();

        private ProcessState p1 = new ProcessState(1, 150, 4);
        private ProcessState p2 = new ProcessState(2, 350, 7);
        private ProcessState p3 = new ProcessState(3, 550, 3);

        private ProcessState[] processes;
        private bool showTheoryOverlay = false;
        private const int ProcessCardWidth = 160;
        private const int ProcessCardMargin = 20;
        private const int TimelinePadding = 40;
        private const int CanvasTopPadding = 200;
        private const int CanvasBottomPadding = 220;
        private const int OverlayDefaultWidth = 520;
        private const int OverlayDefaultHeight = 155;
        private int TimelineLeft => ProcessCardMargin + ProcessCardWidth + TimelinePadding;
        private int TimelineRight => Math.Max(TimelineLeft + 700, (pnlSimulation?.Width ?? Width) - TimelinePadding);

        public Form1()
        {
            InitializeComponent();
            processes = new ProcessState[] { p1, p2, p3 };
            UpdateProcessPositions();

            // Para evitar parpadeo
            typeof(Panel).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, 
                null, pnlSimulation, new object[] { true });

            Resize += (s, e) => AdjustLayout();
            pnlSimulation.Resize += (s, e) =>
            {
                UpdateProcessPositions();
                pnlSimulation.Invalidate();
            };
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            if (splitContainer1.Width <= 0)
            {
                return;
            }
            int reservedWidth = Math.Max(360, splitContainer1.Panel2MinSize); // ancho deseado para controles
            int desiredPanel1 = Math.Max(600, splitContainer1.Width - reservedWidth);
            desiredPanel1 = Math.Min(desiredPanel1, splitContainer1.Width - splitContainer1.Panel2MinSize);
            desiredPanel1 = Math.Max(0, desiredPanel1);
            if (desiredPanel1 <= splitContainer1.Width && desiredPanel1 != splitContainer1.SplitterDistance)
            {
                splitContainer1.SplitterDistance = desiredPanel1;
            }
        }

        private void UpdateProcessPositions()
        {
            if (pnlSimulation == null || processes == null || processes.Length == 0)
            {
                return;
            }

            int topMargin = CanvasTopPadding;
            int bottomMargin = CanvasBottomPadding;
            int usableHeight = Math.Max(100, pnlSimulation.Height - topMargin - bottomMargin);
            int spacing = processes.Length > 1 ? usableHeight / (processes.Length - 1) : 0;

            for (int i = 0; i < processes.Length; i++)
            {
                processes[i].Y = topMargin + (processes.Length > 1 ? spacing * i : usableHeight / 2);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            currentTick++;
            CheckMessageArrivals();
            pnlSimulation.Invalidate();
        }

        private void CheckMessageArrivals()
        {
            var arrived = messages.Where(m => !m.Delivered && currentTick >= m.ReceiveTime).ToList();
            List<MessageEvent> newPending = new List<MessageEvent>();

            foreach (var m in arrived)
            {
                m.Delivered = true;
                ProcessMessage(m, newPending);
            }

            if (newPending.Count > 0)
            {
                messages.AddRange(newPending);
            }
        }

        private void ProcessMessage(MessageEvent m, List<MessageEvent> newPending)
        {
            ProcessState receiver = processes.First(p => p.Id == m.ReceiverId);

            if (m.IsMarker)
            {
                if (!receiver.RecordedState)
                {
                    receiver.RecordedState = true;
                    receiver.CaptureLocalState();
                    receiver.ChannelRecorded[m.SenderId] = true;
                    nodeEvents.Add(new NodeEvent
                    {
                        Tick = currentTick,
                        ProcessId = receiver.Id,
                        Color = Color.Red,
                        Description = "State Recorded",
                        Annotation = $"P{receiver.Id}: estado local capturado = {receiver.RecordedLocalValue}",
                        AnnotationAbove = true
                    });

                    foreach (var other in processes.Where(p => p.Id != receiver.Id))
                    {
                        var marker = new MessageEvent
                        {
                            SenderId = receiver.Id,
                            ReceiverId = other.Id,
                            SendTime = currentTick,
                            ReceiveTime = currentTick + 80,
                            IsMarker = true
                        };
                        newPending.Add(marker);
                    }
                }
                else
                {
                    receiver.ChannelRecorded[m.SenderId] = true;
                }
                return;
            }

            receiver.LocalValue++;

            if (receiver.RecordedState && !receiver.ChannelRecorded.ContainsKey(m.SenderId))
            {
                nodeEvents.Add(new NodeEvent
                {
                    Tick = m.ReceiveTime,
                    ProcessId = receiver.Id,
                    Color = Color.Orange,
                    Description = "Channel Msg",
                    Annotation = $"Mensaje de P{m.SenderId} quedó en tránsito",
                    AnnotationAbove = false
                });
            }
            else
            {
                nodeEvents.Add(new NodeEvent
                {
                    Tick = m.ReceiveTime,
                    ProcessId = receiver.Id,
                    Color = Color.MediumSeaGreen,
                    Description = "Receive Msg",
                    Annotation = $"P{receiver.Id} aplica el mensaje → estado = {receiver.LocalValue}",
                    AnnotationAbove = false
                });
            }
        }

        private int GetTickX(long tick)
        {
            return TimelineRight - (int)((currentTick - tick) * 3);
        }

        private void pnlSimulation_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int timelineLeft = TimelineLeft;
            int timelineRight = TimelineRight;
            int overlayTop = Math.Max(CanvasTopPadding / 2, processes.First().Y - 120);
            int overlayBottom = processes.Last().Y + CanvasBottomPadding / 2;

            using (var bg = new System.Drawing.Drawing2D.LinearGradientBrush(pnlSimulation.ClientRectangle, Color.White, Color.FromArgb(245, 249, 255), 90f))
            {
                g.FillRectangle(bg, pnlSimulation.ClientRectangle);
            }

            foreach (var process in processes)
            {
                DrawProcessCard(g, process);
            }

            var recNodes = nodeEvents.Where(ev => ev.Description == "State Recorded")
                                     .OrderBy(ev => processes.First(p => p.Id == ev.ProcessId).Y)
                                     .ToList();
            if (recNodes.Count > 0)
            {
                var cutPoints = new List<Point>
                {
                    new Point(Math.Min(GetTickX(recNodes.First().Tick), timelineRight), overlayTop)
                };

                foreach (var ev in recNodes)
                {
                    cutPoints.Add(new Point(Math.Min(GetTickX(ev.Tick), timelineRight), processes.First(p => p.Id == ev.ProcessId).Y));
                }

                cutPoints.Add(new Point(Math.Min(GetTickX(recNodes.Last().Tick), timelineRight), overlayBottom));

                var poly = new List<Point>
                {
                    new Point(timelineLeft - 60, overlayTop - 40)
                };
                poly.AddRange(cutPoints);
                poly.Add(new Point(timelineLeft - 60, overlayBottom + 40));

                using (var fillBrush = new SolidBrush(Color.FromArgb(70, 111, 207, 151)))
                {
                    g.FillPolygon(fillBrush, poly.ToArray());
                }

                using (var cutPen = new Pen(Color.FromArgb(210, 211, 47, 47), 4) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    g.DrawLines(cutPen, cutPoints.ToArray());
                }

                if (showTheoryOverlay)
                {
                    var overlayRect = GetOverlayBounds(timelineLeft, timelineRight, overlayTop, OverlayDefaultWidth, OverlayDefaultHeight);
                    DrawNarrativeOverlay(g, new[]
                    {
                        "Estado global = corte consistente",
                        "Zona verde = pasado registrado",
                        "Rojos: estados locales guardados",
                        "Naranjas: mensajes atrapados"
                    }, overlayRect);
                }
            }
            else if (showTheoryOverlay)
            {
                var overlayRect = GetOverlayBounds(timelineLeft, timelineRight, overlayTop, 420, 140);
                DrawNarrativeOverlay(g, new[]
                {
                    "1. Genera algunos mensajes azules",
                    "2. Presiona 'Snapshot' desde P1",
                    "3. Observa cómo la línea roja captura estados"
                }, overlayRect);
            }

            using (var gridPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
            {
                for (int x = timelineLeft; x < timelineRight; x += 90)
                {
                    g.DrawLine(gridPen, x, processes.First().Y - 120, x, processes.Last().Y + 140);
                }
            }

            foreach (var p in processes)
            {
                using (var linePen = new Pen(Color.FromArgb(180, 70, 70, 70), 3))
                {
                    g.DrawLine(linePen, timelineLeft, p.Y, timelineRight, p.Y);
                }
            }

            foreach (var m in messages)
            {
                int startX = GetTickX(m.SendTime);
                int startY = processes.First(p => p.Id == m.SenderId).Y;
                int endX, endY;

                using (Pen msgPen = m.IsMarker ? new Pen(Color.Red, 3) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash } : new Pen(Color.Blue, 3))
                {
                    Brush msgBrush = m.IsMarker ? Brushes.Red : Brushes.Blue;

                    if (currentTick < m.ReceiveTime)
                    {
                        endX = GetTickX(currentTick);
                        int targetY = processes.First(p => p.Id == m.ReceiverId).Y;
                        endY = startY + (int)((targetY - startY) * ((double)(currentTick - m.SendTime) / (m.ReceiveTime - m.SendTime)));
                    }
                    else
                    {
                        endX = GetTickX(m.ReceiveTime);
                        endY = processes.First(p => p.Id == m.ReceiverId).Y;
                    }

                    if (startX < timelineRight && endX > timelineLeft - 150)
                    {
                        g.DrawLine(msgPen, startX, startY, endX, endY);
                        g.FillEllipse(msgBrush, startX - 5, startY - 5, 10, 10);
                        g.FillEllipse(msgBrush, endX - 6, endY - 6, 12, 12);
                    }
                }
            }

            foreach (var ev in nodeEvents)
            {
                int x = GetTickX(ev.Tick);
                int y = processes.First(p => p.Id == ev.ProcessId).Y;

                if (x < timelineLeft - 200 || x > timelineRight + 60)
                {
                    continue;
                }

                int size = ev.Color == Color.Orange ? 26 : ev.Color == Color.Red ? 22 : 16;
                using (Brush b = new SolidBrush(ev.Color))
                using (Pen borderPen = new Pen(Color.White, 2))
                {
                    g.FillEllipse(b, x - (size / 2), y - (size / 2), size, size);
                    g.DrawEllipse(borderPen, x - (size / 2), y - (size / 2), size, size);
                }

                if (!string.IsNullOrWhiteSpace(ev.Annotation))
                {
                    DrawCallout(g, ev.Annotation, x, y, ev.Color, ev.AnnotationAbove);
                }
            }

            DrawLegend(g, timelineLeft, timelineRight);

            using (var nowPen = new Pen(Color.Gray, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
            {
                g.DrawLine(nowPen, timelineRight, overlayTop - 30, timelineRight, processes.Last().Y + 150);
            }

            int labelWidth = 260;
            int labelHeight = 34;
            int labelX = Math.Max(timelineLeft, timelineRight - labelWidth - 20);
            int labelY = Math.Max(15, CanvasTopPadding - labelHeight - 70);
            var labelRect = new Rectangle(labelX, labelY, labelWidth, labelHeight);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
            {
                var shadowRect = new Rectangle(labelRect.X + 3, labelRect.Y + 3, labelRect.Width, labelRect.Height);
                g.FillRectangle(shadowBrush, shadowRect);
            }
            using (var labelBrush = new SolidBrush(Color.FromArgb(252, 252, 252)))
            using (var labelPen = new Pen(Color.Gray, 1))
            using (var labelFont = new Font("Segoe UI", 10, FontStyle.Bold))
            {
                g.FillRectangle(labelBrush, labelRect);
                g.DrawRectangle(labelPen, labelRect);
                g.DrawString("LÍNEA DE TIEMPO ACTUAL", labelFont, Brushes.DimGray, labelRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawNarrativeOverlay(Graphics g, string[] lines, Rectangle bounds)
        {
            const int cornerRadius = 18;
            const int padding = 18;
            const int headerHeight = 36;
            Rectangle shadowRect = new Rectangle(bounds.Left + 4, bounds.Top + 6, bounds.Width, bounds.Height);

            using (GraphicsPath shadowPath = RoundedRect(shadowRect, cornerRadius))
            using (GraphicsPath cardPath = RoundedRect(bounds, cornerRadius))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(35, 0, 0, 0)))
            using (var gradient = new LinearGradientBrush(bounds, Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 238, 248, 240), LinearGradientMode.Vertical))
            using (var borderPen = new Pen(Color.FromArgb(160, 46, 136, 86), 2))
            using (var headerBrush = new SolidBrush(Color.FromArgb(255, 218, 240, 224)))
            using (var titleFont = new Font("Segoe UI", 15, FontStyle.Bold))
            using (var bodyFont = new Font("Segoe UI", 11, FontStyle.Regular))
            using (var titleBrush = new SolidBrush(Color.FromArgb(24, 82, 57)))
            using (var textBrush = new SolidBrush(Color.FromArgb(70, 70, 70)))
            using (var accentBrush = new SolidBrush(Color.FromArgb(255, 46, 136, 86)))
            {
                g.FillPath(shadowBrush, shadowPath);
                g.FillPath(gradient, cardPath);
                g.DrawPath(borderPen, cardPath);

                var headerRect = new Rectangle(bounds.Left + padding, bounds.Top + padding, bounds.Width - (padding * 2), headerHeight);
                g.FillRectangle(headerBrush, headerRect);

                if (lines.Length > 0)
                {
                    g.DrawString(lines[0], titleFont, titleBrush, headerRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }

                int y = headerRect.Bottom + 10;
                for (int i = 1; i < lines.Length; i++)
                {
                    var lineRect = new Rectangle(bounds.Left + padding + 16, y, bounds.Width - (padding * 2) - 16, bodyFont.Height + 4);
                    g.FillEllipse(accentBrush, lineRect.Left - 12, lineRect.Top + (lineRect.Height / 2) - 3, 6, 6);
                    g.DrawString(lines[i], bodyFont, textBrush, lineRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                    y += bodyFont.Height + 10;
                }
            }
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Rectangle GetOverlayBounds(int timelineLeft, int timelineRight, int overlayTop, int width = 420, int height = 120)
        {
            int margin = 30;
            int desiredX = timelineLeft + margin;
            int maxX = Math.Max(desiredX, timelineRight - width - margin);
            desiredX = Math.Min(desiredX, maxX);

            int desiredY = Math.Max(20, CanvasTopPadding - height - 60);
            if (desiredY + height > pnlSimulation.Height - margin)
            {
                desiredY = Math.Max(margin, overlayTop + margin);
            }

            return new Rectangle(desiredX, desiredY, width, height);
        }

        private void DrawProcessCard(Graphics g, ProcessState process)
        {
            int cardWidth = ProcessCardWidth;
            int cardHeight = 130;
            int cardX = ProcessCardMargin;
            int cardY = process.Y - cardHeight / 2;
            var cardRect = new Rectangle(cardX, cardY, cardWidth, cardHeight);

            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(cardRect, Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 235, 239, 245), 90f))
            {
                g.FillRectangle(brush, cardRect);
            }

            using (var border = new Pen(process.RecordedState ? Color.Firebrick : Color.SlateGray, process.RecordedState ? 3 : 1))
            {
                g.DrawRectangle(border, cardRect);
            }

            using (var titleFont = new Font("Segoe UI", 16, FontStyle.Bold))
            {
                g.DrawString($"P{process.Id}", titleFont, Brushes.Black, cardRect.Left + 12, cardRect.Top + 6);
            }

            using (var valueFont = new Font("Segoe UI", 11, FontStyle.Bold))
            {
                g.DrawString($"Estado local: {process.LocalValue}", valueFont, Brushes.DimGray, cardRect.Left + 12, cardRect.Top + 52);
            }

            if (process.RecordedLocalValue.HasValue)
            {
                var badgeRect = new Rectangle(cardRect.Left + 12, cardRect.Bottom - 38, cardWidth - 24, 28);
                using (var badgeBrush = new SolidBrush(Color.FromArgb(255, 255, 244, 244)))
                using (var badgePen = new Pen(Color.Firebrick, 1))
                using (var badgeFont = new Font("Segoe UI", 9, FontStyle.Bold))
                {
                    g.FillRectangle(badgeBrush, badgeRect);
                    g.DrawRectangle(badgePen, badgeRect);
                    g.DrawString($"Snapshot = {process.RecordedLocalValue}", badgeFont, Brushes.Firebrick, badgeRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
            }
        }

        private void DrawCallout(Graphics g, string text, int anchorX, int anchorY, Color accent, bool placeAbove)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            const int width = 260;
            const int height = 54;
            int rectX = anchorX - (width / 2);
            rectX = Math.Clamp(rectX, TimelineLeft - 60, TimelineRight - width);
            int rectY = placeAbove ? anchorY - height - 34 : anchorY + 30;
            var rect = new Rectangle(rectX, rectY, width, height);

            using (var bg = new SolidBrush(Color.FromArgb(235, 255, 255, 255)))
            using (var pen = new Pen(accent, 2))
            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var brush = new SolidBrush(accent))
            {
                g.FillRectangle(bg, rect);
                g.DrawRectangle(pen, rect);
                g.DrawString(text, font, brush, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                int connectorY = placeAbove ? rect.Bottom : rect.Top;
                g.DrawLine(pen, anchorX, anchorY, anchorX, connectorY);
            }
        }

        private void DrawLegend(Graphics g, int left, int right)
        {
            int width = Math.Max(380, right - left - 40);
            var rect = new Rectangle(left, pnlSimulation.Height - 90, width, 70);
            using (var bg = new SolidBrush(Color.FromArgb(235, 248, 248, 248)))
            using (var pen = new Pen(Color.Gainsboro, 1))
            {
                g.FillRectangle(bg, rect);
                g.DrawRectangle(pen, rect);
            }

            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            {
                int iconY = rect.Top + 22;
                int cursorX = rect.Left + 16;
                DrawLegendCircle(g, font, cursorX, iconY, Color.Red, "Estado local capturado");
                cursorX += 190;
                DrawLegendCircle(g, font, cursorX, iconY, Color.Orange, "Mensaje en tránsito");
                cursorX += 170;

                using (var markerPen = new Pen(Color.Red, 3) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    g.DrawLine(markerPen, cursorX, iconY + 8, cursorX + 40, iconY + 8);
                }
                g.DrawString("Marcador", font, Brushes.DimGray, cursorX + 48, iconY + 1);
                cursorX += 160;

                using (var msgPen = new Pen(Color.Blue, 3))
                {
                    g.DrawLine(msgPen, cursorX, iconY + 8, cursorX + 40, iconY - 2);
                }
                g.DrawString("Mensaje normal", font, Brushes.DimGray, cursorX + 48, iconY + 1);
            }
        }

        private void DrawLegendCircle(Graphics g, Font font, int x, int y, Color color, string text)
        {
            using (var circleBrush = new SolidBrush(color))
            using (var borderPen = new Pen(Color.WhiteSmoke, 1))
            {
                g.FillEllipse(circleBrush, x, y, 18, 18);
                g.DrawEllipse(borderPen, x, y, 18, 18);
            }

            g.DrawString(text, font, Brushes.DimGray, x + 24, y - 2);
        }

        private void btnP1Send_Click(object sender, EventArgs e)
        {
            SendMessage(1, 2);
        }

        private void btnP2Send_Click(object sender, EventArgs e)
        {
            SendMessage(2, 3);
        }

        private void btnP3Send_Click(object sender, EventArgs e)
        {
            SendMessage(3, 1);
        }

        private void SendMessage(int from, int to)
        {
            var sender = processes.First(p => p.Id == from);
            sender.LocalValue++;
            var msg = new MessageEvent
            {
                SenderId = from,
                ReceiverId = to,
                SendTime = currentTick,
                ReceiveTime = currentTick + 60,
                IsMarker = false
            };
            messages.Add(msg);
            nodeEvents.Add(new NodeEvent
            {
                Tick = currentTick,
                ProcessId = from,
                Color = Color.CadetBlue,
                Description = "Send Msg",
                Annotation = $"P{from} genera evento local → estado = {sender.LocalValue}",
                AnnotationAbove = true
            });
        }

        private void btnSnapshot_Click(object sender, EventArgs e)
        {
            if (p1.RecordedState)
            {
                return;
            }

            p1.RecordedState = true;
            p1.CaptureLocalState();
            nodeEvents.Add(new NodeEvent
            {
                Tick = currentTick,
                ProcessId = 1,
                Color = Color.Red,
                Description = "State Recorded",
                Annotation = $"P1 captura su estado local = {p1.RecordedLocalValue}",
                AnnotationAbove = true
            });

            messages.Add(new MessageEvent { SenderId = 1, ReceiverId = 2, SendTime = currentTick, ReceiveTime = currentTick + 80, IsMarker = true });
            messages.Add(new MessageEvent { SenderId = 1, ReceiverId = 3, SendTime = currentTick, ReceiveTime = currentTick + 120, IsMarker = true });
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            showTheoryOverlay = !showTheoryOverlay;
            btnInfo.Text = showTheoryOverlay ? "Ocultar notas visuales" : "Mostrar notas visuales";
            pnlSimulation.Invalidate();
        }
    }

    public class ProcessState
    {
        public int Id { get; set; }
        public int Y { get; set; }
        public bool RecordedState { get; set; } = false;
        public int LocalValue { get; set; }
        public int? RecordedLocalValue { get; set; }
        // True cuando el marcador de este Id ha sido recibido
        public Dictionary<int, bool> ChannelRecorded { get; set; } = new Dictionary<int, bool>();

        public ProcessState(int id, int y, int initialValue = 0)
        {
            Id = id;
            Y = y;
            LocalValue = initialValue;
        }

        public void CaptureLocalState()
        {
            RecordedLocalValue = LocalValue;
        }
    }

    public class MessageEvent
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public long SendTime { get; set; }
        public long ReceiveTime { get; set; }
        public bool IsMarker { get; set; }
        public bool Delivered { get; set; } = false;
    }

    public class NodeEvent
    {
        public long Tick { get; set; }
        public int ProcessId { get; set; }
        public Color Color { get; set; }
        public string Description { get; set; }
        public string Annotation { get; set; }
        public bool AnnotationAbove { get; set; } = true;
    }
}
