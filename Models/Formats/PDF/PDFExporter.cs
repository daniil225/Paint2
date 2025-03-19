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
using System.Numerics;

namespace Formats.PDF;

public class PDFExporter : IExportFormat
{
    public void SaveTo(IExportSnapshot snapshot, string destinationPath)
    {
        var pDFSnapshot = snapshot as PDFSnapshot;

        using (PdfWriter writer = new PdfWriter(destinationPath))
        using (PdfDocument pdf = new PdfDocument(writer))
        {
            PdfPage page = pdf.AddNewPage(new PageSize(pDFSnapshot._width, pDFSnapshot._height));

            // Создаём PdfCanvas для рисования
            PdfCanvas canvas = new PdfCanvas(page);

            SaveElement(pDFSnapshot._tree, canvas);
        }
        
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

    private void SaveElement(PDFPath pDFPath, PdfCanvas canvas)
    {
        canvas.SaveState();
        ApplyTransformations(pDFPath, canvas);
        ApplyBrush(pDFPath, canvas);

        var elements = pDFPath.Elements;

        Paint2.ViewModels.Utils.Point CurrentPoint = null;

        foreach (var element in elements)
        {
            if(element is PathArcTo pathArc)
            {
                CurrentPoint = SavePathElement(pathArc, canvas, CurrentPoint);
            }
            else if(element is PathClose close)
            {
                SavePathElement(close, canvas);
            }
            else
                CurrentPoint = SavePathElement((dynamic)element, canvas);
        }

        canvas.Stroke();

        canvas.RestoreState();
    }

    private Paint2.ViewModels.Utils.Point SavePathElement(PathMoveTo move, PdfCanvas canvas)
    {
        Paint2.ViewModels.Utils.Point dest = move.dest;
        canvas.MoveTo(dest.X, dest.Y);
        return dest;
    }

    private Paint2.ViewModels.Utils.Point SavePathElement(PathLineTo line, PdfCanvas canvas)
    {
        Paint2.ViewModels.Utils.Point dest = line.dest;
        canvas.LineTo(dest.X, dest.Y);
        return dest;
    }

    private Paint2.ViewModels.Utils.Point SavePathElement(PathArcTo arc, PdfCanvas canvas, Paint2.ViewModels.Utils.Point curPoint)
    {
        double angle_rad = arc.xAxisRotation / 180 * Math.PI;
        var sinA = Math.Sin(angle_rad);
        var cosA = Math.Cos(angle_rad);

        //// Large arc flag
        bool fLarge = arc.largeArcFlag;

        //// Sweep flag
        var fS = arc.sweepDirection == Avalonia.Media.SweepDirection.Clockwise ? true : false;

        var dest = arc.dest;

        var radiusX = arc.radiusX;
        var radiusY = arc.radiusY;
        var x1 = curPoint.X;
        var y1 = curPoint.Y;
        var x2 = dest.X;
        var y2 = dest.Y;

        //Compute (x1′, y1′)/

        //// Median between Start and End
        var midPointX = (x1 + x2) / 2;
        var midPointY = (y1 + y2) / 2;

        var x1p = (cosA * midPointX) + (sinA * midPointY);
        var y1p = (cosA * midPointY) - (sinA * midPointX);

         // Step 2: Compute (cx′, cy′)

        var rxry_2 = Math.Pow(radiusX, 2) * Math.Pow(radiusY, 2);
        var rxy1p_2 = Math.Pow(radiusX, 2) * Math.Pow(y1p, 2);
        var ryx1p_2 = Math.Pow(radiusY, 2) * Math.Pow(x1p, 2);

        var sqrt = Math.Sqrt(Math.Abs(rxry_2 - rxy1p_2 - ryx1p_2) / (rxy1p_2 + ryx1p_2));
        if (fLarge == fS)
            sqrt = -sqrt;

        var cXP = sqrt * (radiusX * y1p / radiusY);
        var cYP = sqrt * -(radiusY * x1p / radiusX);

         // Step 3: Compute (cx, cy) from (cx′, cy′)

        var cX = (cosA * cXP) - (sinA * cYP) + ((x1 + x2) / 2);
        var cY = (sinA * cXP) + (cosA * cYP) + ((y1 + y2) / 2);

         // Step 4: Compute θ1 and dθ

        var x1pcxp_rx = (x1p - cXP) / radiusX;
        var y1pcyp_ry = (y1p - cYP) / radiusY;
        var vector1 = new Paint2.ViewModels.Utils.Point(1d, 0d);
        var vector2 = new Paint2.ViewModels.Utils.Point(x1pcxp_rx, y1pcyp_ry);

        var angle = Math.Acos(((vector1.X * vector2.X) + (vector1.Y * vector2.Y)) / (Math.Sqrt((vector1.X * vector1.X) + (vector1.Y * vector1.Y)) * Math.Sqrt((vector2.X * vector2.X) + (vector2.Y * vector2.Y)))) * (180 / Math.PI);

        if (((vector1.X * vector2.Y) - (vector1.Y * vector2.X)) < 0)
        {
            angle = angle * -1;
        }

        var vector3 = new Paint2.ViewModels.Utils.Point(x1pcxp_rx, y1pcyp_ry);
        var vector4 = new Paint2.ViewModels.Utils.Point((-x1p - cXP) / radiusX, (-y1p - cYP) / radiusY);

        var extent = (Math.Acos(((vector3.X * vector4.X) + (vector3.Y * vector4.Y)) / Math.Sqrt((vector3.X * vector3.X) + (vector3.Y * vector3.Y)) * Math.Sqrt((vector4.X * vector4.X) + (vector4.Y * vector4.Y))) * (180 / Math.PI)) % 360;

        if (((vector3.X * vector4.Y) - (vector3.Y * vector4.X)) < 0)
        {
            extent = extent * -1;
        }

        if (fS == true && extent < 0)
        {
            extent = extent + 360;
        }

        if (fS == false && extent > 0)
        {
            extent = extent - 360;
        }

        var rectLL_X = cX - radiusX;
        var rectLL_Y = cY - radiusY;

        var rectUR_X = cX + radiusX;
        var rectUR_Y = cY + radiusY;

        canvas.Arc(rectLL_X, rectLL_Y, rectUR_X, rectUR_Y, angle, extent);

        return dest;
    }

    private Paint2.ViewModels.Utils.Point SavePathElement(PathCubicBezierTo curve, PdfCanvas canvas)
    {
        canvas.CurveTo(curve.controlPoint1.X,
                       curve.controlPoint1.Y,
                       curve.controlPoint2.X,
                       curve.controlPoint2.Y,
                       curve.dest.X,
                       curve.dest.Y);
        return curve.dest;
    }

    private void SavePathElement(PathClose close, PdfCanvas canvas)
    {
        canvas.ClosePath();
        canvas.Fill();
    }

    private void SaveElement(PDFGroup group, PdfCanvas canvas)
    {
        List<PDFElement> elements = group.Children;
        canvas.SaveState();
        ApplyTransformations(group, canvas);
        ApplyBrush(group, canvas);

        foreach (PDFElement element in elements)
            SaveElement((dynamic)element,canvas);

        canvas.RestoreState();
    }
}



