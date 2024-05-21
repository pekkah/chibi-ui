using System;
using System.Collections.Generic;

namespace Chibi.Ui.Experimental;

public static class PixelComparer
{
    public static List<Rect> FindDifferenceRects(ReadOnlySpan<ushort> oldPixels, ReadOnlySpan<ushort> newPixels, int width, int height)
    {
        if (oldPixels.Length != newPixels.Length || oldPixels.Length != width * height)
            throw new ArgumentException("Array sizes or dimensions are incorrect");

        var rects = new List<Rect>();
        Span<bool> diffMatrix = stackalloc bool[width * height];

        // Identify differences and mark them in a boolean matrix
        for (int i = 0; i < oldPixels.Length; i++)
        {
            if (oldPixels[i] != newPixels[i])
            {
                diffMatrix[i] = true;
            }
        }

        // Find rectangles in the boolean matrix
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                if (diffMatrix[index])
                {
                    // Find width of the rectangle
                    int rectWidth = 1;
                    while (x + rectWidth < width && diffMatrix[index + rectWidth])
                    {
                        rectWidth++;
                    }

                    // Find height of the rectangle
                    int rectHeight = 1;
                    bool isRect = true;
                    while (y + rectHeight < height && isRect)
                    {
                        for (int i = 0; i < rectWidth; i++)
                        {
                            if (!diffMatrix[index + i + rectHeight * width])
                            {
                                isRect = false;
                                break;
                            }
                        }
                        if (isRect)
                        {
                            rectHeight++;
                        }
                    }

                    // Add the found rectangle
                    rects.Add(new Rect(x, y, rectWidth, rectHeight));

                    // Mark the rectangle area as processed
                    for (int dy = 0; dy < rectHeight; dy++)
                    {
                        for (int dx = 0; dx < rectWidth; dx++)
                        {
                            diffMatrix[index + dx + dy * width] = false;
                        }
                    }

                    x += rectWidth - 1; // Skip processed pixels
                }
            }
        }

        return rects;
    }
}