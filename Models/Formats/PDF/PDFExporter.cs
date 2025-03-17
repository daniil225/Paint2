using Formats.Svg;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf;
using Paint2.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using ExCSS;
using Avalonia.Controls.Shapes;
using Paint2.ViewModels.Utils;

namespace Formats.PDF;

public class PDFExporter : IExportFormat
{
    public void SaveTo(IExportSnapshot snapshot, string destinationPath)
    {
        var pDFSnapshot = snapshot as PDFSnapshot;

        PdfWriter writer = new PdfWriter(destinationPath);
        PdfDocument pdf = new PdfDocument(writer);
        
        // Добавляем страницу
        PdfPage page = pdf.AddNewPage(new PageSize(pDFSnapshot._width, pDFSnapshot._height));

        // Создаём PdfCanvas для рисования
        PdfCanvas canvas = new PdfCanvas(page);

        SaveElement(pDFSnapshot._tree,canvas);
    }

    private void ApplyTransformations(PDFElement pdfElement, PdfCanvas canvas)
    {
        if (pdfElement.Transforms == null) return;

        ITransform[] Transforms = pdfElement.Transforms.ToArray();

        foreach ( ITransform transform in Transforms )
        {
            switch(transform)
            {
                case (Translate shift):
                    canvas.ConcatMatrix(1, 0, 0, 1, shift.X, shift.Y);
                    break;
                case (Rotate rotation):
                    double rad = Math.PI / 180 * rotation.Angle;
                    double cos = Math.Cos(rad);
                    double sin = Math.Sin(rad);

                    if(rotation.Pivot == null)
                    {
                        canvas.ConcatMatrix(cos, sin, -sin, cos, 0, 0);
                        break;
                    }
                        
                    canvas.ConcatMatrix(1, 0, 0, 1, rotation.Pivot.X, rotation.Pivot.Y);
                    canvas.ConcatMatrix(cos, sin, -sin, cos, 0, 0);
                    canvas.ConcatMatrix(1, 0, 0, 1, -rotation.Pivot.X, -rotation.Pivot.Y);

                    break;
            }
        }
        //canvas.SaveState();
        //canvas.ConcatMatrix()
    }

    private void ApplyBrush(PDFElement pdfElement, PdfCanvas canvas)
    {
        if (pdfElement.Brush == null)
            return;
        Formats.Brush Brush = pdfElement.Brush;
        canvas.SetStrokeColorRgb(Brush.Stroke.R, Brush.Stroke.G, Brush.Stroke.B);

        canvas.SetLineWidth((float)Brush.StrokeWidth);

        canvas.SetFillColorRgb(Brush.Fill.R, Brush.Fill.G, Brush.Fill.B);

        if (Brush.Dash == null)
            return;
        canvas.SetLineDash((float)Brush.Dash.Value.Length, (float)Brush.Dash.Value.Gap);
    }

    private void SaveElement(PDFRect rect, PdfCanvas canvas)
    {
        canvas.SaveState();
        ApplyTransformations(rect, canvas);
        ApplyBrush(rect, canvas);
        canvas.Rectangle(rect.Position.X, rect.Position.Y, rect.Width, rect.Height);
        canvas.FillStroke();
        canvas.RestoreState();
    }

    private void SaveElement(PDFCircle circle, PdfCanvas canvas)
    {
        canvas.SaveState();
        ApplyTransformations(circle, canvas);
        ApplyBrush(circle, canvas);
        canvas.Circle(circle.Center.X, circle.Center.Y, circle.Radius);
        canvas.FillStroke();
        canvas.RestoreState();
    }

    private void SaveElement(PDFLine line, PdfCanvas canvas)
    {
        canvas.SaveState();
        ApplyTransformations(line, canvas);
        ApplyBrush(line, canvas);
        canvas.MoveTo(line.Start.X, line.Start.Y);
        canvas.LineTo(line.End.X, line.End.Y);
        canvas.Stroke();
        canvas.RestoreState();
    }

    private void SaveElement(PDFPolyline polyline, PdfCanvas canvas)
    {
        canvas.SaveState();
        ApplyTransformations(polyline, canvas);
        ApplyBrush(polyline, canvas);
        Paint2.ViewModels.Utils.Point[] points = polyline.Points.ToArray();
        canvas.MoveTo(points[0].X, points[0].Y);
        for(int i = 1; i < points.Length; i++)
            canvas.LineTo(points[i].X, points[i].Y);
        canvas.Stroke();
        canvas.RestoreState();
    }

    private void SaveElement(PDFPolygon polygon, PdfCanvas canvas)
    {
        canvas.SaveState();
        ApplyTransformations(polygon, canvas);
        ApplyBrush(polygon, canvas);
        Paint2.ViewModels.Utils.Point[] points = polygon.Points.ToArray();

        canvas.MoveTo(points[0].X, points[0].Y);

        for (int i = 1; i < points.Length; i++)
            canvas.LineTo(points[i].X, points[i].Y);

        canvas.ClosePathFillStroke();
        canvas.RestoreState();
    }

    private void SavePath()
    {

    }

    private void SaveElement(PDFGroup group, PdfCanvas canvas)
    {

    }
}



