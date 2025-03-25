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
using Avalonia.Controls.Shapes;
using System.Numerics;
using iText.Kernel.Pdf.Extgstate;
using Avalonia.Media;
using Paint2.ViewModels.Utils;
using Point = Paint2.ViewModels.Utils.Point;
using Avalonia;

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

            canvas.ConcatMatrix(1, 0, 0, -1, 0, pDFSnapshot._height);

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
    }

    private void ApplyBrush(PDFElement pdfElement, PdfCanvas canvas)
    {
        if (pdfElement.Brush == null)
            return;
        Formats.Brush Brush = pdfElement.Brush;
        canvas.SetStrokeColorRgb(Brush.Stroke.R/255f, Brush.Stroke.G/255f, Brush.Stroke.B/255f);

        canvas.SetLineWidth((float)Brush.StrokeWidth);

        canvas.SetFillColorRgb(Brush.Fill.R/255f, Brush.Fill.G/255f, Brush.Fill.B/255f);

        float[] dashArray = Brush.Dash.Select(d => (float)d).ToArray();

        if (dashArray.Count() != 0)
            canvas.SetLineDash(dashArray, 0);
        else
            canvas.SetLineDash(0);


       PdfExtGState gState = new();

        gState.SetFillOpacity(Brush.Fill.A/255f);

        gState.SetStrokeOpacity(Brush.Stroke.A/255f);

        canvas.SetExtGState(gState);
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

        Point CurrentPoint = null;

        bool close_f = false;

        foreach (var element in elements)
        {
            if(element is PathArcTo pathArc)
            {
                CurrentPoint = SavePathElement(pathArc, canvas, CurrentPoint);
            }
            else if(element is PathClose close)
            {
                close_f = true;
            }
            else
                CurrentPoint = SavePathElement((dynamic)element, canvas);
        }

        switch(close_f)
        {
            case (true):
                canvas.ClosePath();
                canvas.FillStroke();
                break;
            case (false):
                canvas.Stroke();
                break;
        }

        canvas.RestoreState();
    }

    private Point SavePathElement(PathMoveTo move, PdfCanvas canvas)
    {
        Paint2.ViewModels.Utils.Point dest = move.dest;
        canvas.MoveTo(dest.X, dest.Y);
        return dest;
    }

    private Point SavePathElement(PathLineTo line, PdfCanvas canvas)
    {
        Paint2.ViewModels.Utils.Point dest = line.dest;
        canvas.LineTo(dest.X, dest.Y);
        return dest;
    }

    private Point SavePathElement(PathArcTo arc, PdfCanvas canvas, Point curPoint)
    {
        
        double sinA = Math.Sin(arc.xAxisRotation*Math.PI/180);
        double cosA = Math.Cos(arc.xAxisRotation * Math.PI / 180);

        //// Large arc flag
        double fA = arc.largeArcFlag == true ? 1 : 0;

        //// Sweep flag
        double fS = arc.sweepDirection == SweepDirection.CounterClockwise ? 0 : 1;

        double radiusX = arc.radiusX;
        double radiusY = arc.radiusY;
        double x1 = curPoint.X;
        double y1 = curPoint.Y;
        double x2 = arc.dest.X;
        double y2 = arc.dest.Y;

        /*
         *
         * Step 1: Compute (x1′, y1′)
         * 
         */

        //// Median between Start and End
        double midPointX = (x1 - x2) / 2;
        double midPointY = (y1 - y2) / 2;

        double x1p = (cosA * midPointX) + (sinA * midPointY);
        double y1p = (cosA * midPointY) - (sinA * midPointX);

        /*
         *
         * Step 2: Compute (cx′, cy′)
         * 
         */

        double rxry_2 = Math.Pow(radiusX, 2) * Math.Pow(radiusY, 2);
        double rxy1p_2 = Math.Pow(radiusX, 2) * Math.Pow(y1p, 2);
        double ryx1p_2 = Math.Pow(radiusY, 2) * Math.Pow(x1p, 2);

        double sqrt = Math.Sqrt(Math.Abs(rxry_2 - rxy1p_2 - ryx1p_2) / (rxy1p_2 + ryx1p_2));
        if (fA == fS)
        {
            sqrt = -sqrt;
        }

        double cXP = sqrt * (radiusX * y1p / radiusY);
        double cYP = sqrt * -(radiusY * x1p / radiusX);

        /*
         *
         * Step 3: Compute (cx, cy) from (cx′, cy′)
         * 
         */

        double cX = (cosA * cXP) - (sinA * cYP) + ((x1 + x2) / 2d);
        double cY = (sinA * cXP) + (cosA * cYP) + ((y1 + y2) / 2d);

        /*
         *
         * Step 4: Compute θ1 and Δθ
         * 
         */

        double x1pcxp_rx = (x1p - cXP) / radiusX;
        double y1pcyp_ry = (y1p - cYP) / radiusY;
        Point vector1 = new(1d, 0d);
        Point vector2 = new(x1pcxp_rx, y1pcyp_ry);

        double angle = Math.Acos(((vector1.X * vector2.X) + (vector1.Y * vector2.Y)) / (Math.Sqrt((vector1.X * vector1.X) + (vector1.Y * vector1.Y)) * Math.Sqrt((vector2.X * vector2.X) + (vector2.Y * vector2.Y)))) * (180 / Math.PI);

        if (((vector1.X * vector2.Y) - (vector1.Y * vector2.X)) < 0)
        {
            angle = angle * -1;
        }

        Point vector3 = new(x1pcxp_rx, y1pcyp_ry);
        Point vector4 = new((-x1p - cXP) / radiusX, (-y1p - cYP) / radiusY);

        double extent = (Math.Acos(((vector3.X * vector4.X) + (vector3.Y * vector4.Y)) / Math.Sqrt((vector3.X * vector3.X) + (vector3.Y * vector3.Y)) * Math.Sqrt((vector4.X * vector4.X) + (vector4.Y * vector4.Y))) * (180 / Math.PI)) % 360;

        if (((vector3.X * vector4.Y) - (vector3.Y * vector4.X)) < 0)
        {
            extent = extent * -1;
        }

        if (fS == 1 && extent < 0)
        {
            extent = extent + 360;
        }

        if (fS == 0 && extent > 0)
        {
            extent = extent - 360;
        }
        
        double rectLL_X = cX - radiusX;
        double rectLL_Y = cY - radiusY;

        double rectUR_X = cX + radiusX;
        double rectUR_Y = cY + radiusY;

        IList<double[]> list = PdfCanvas.BezierArc(rectLL_X, rectLL_Y, rectUR_X, rectUR_Y, angle, extent);
        //if (list.IsEmpty())
        //{
        //    return this;
        //}

        List<double[]> listRotated = new List<double[]>(list.Count);

        for(int i =0;i<list.Count;i++)
        {
            listRotated.Add(new double[8]);

            double[] CurrentArray = list[i]; 

            for(int j = 0;j< CurrentArray.Length;j+=2)
            {
                double x = cosA * (CurrentArray[j]-cX) - sinA * (CurrentArray[j + 1]-cY) + cX;
                double y = cosA * (CurrentArray[j + 1]-cY) + sinA * (CurrentArray[j]-cX) + cY;
                CurrentArray[j] = x;
                CurrentArray[j + 1] = y;
            }
            listRotated[i] = CurrentArray;
        }

        double[] array = listRotated[0];
        //canvas.MoveTo(x1, y1);

        for (int i = 0; i < list.Count-1; i++)
        {
            array = listRotated[i];
            canvas.CurveTo(array[2], array[3], array[4], array[5], array[6], array[7]);
        }

        array = listRotated[list.Count - 1];
        canvas.CurveTo(array[2], array[3], array[4], array[5], x2, y2);

        return arc.dest;
    }

    private Point SavePathElement(PathCubicBezierTo curve, PdfCanvas canvas)
    {
        canvas.CurveTo(curve.controlPoint1.X,
                       curve.controlPoint1.Y,
                       curve.controlPoint2.X,
                       curve.controlPoint2.Y,
                       curve.dest.X,
                       curve.dest.Y);
        return curve.dest;
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



