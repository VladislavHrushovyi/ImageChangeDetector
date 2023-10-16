namespace ImageChangeDetector;

public readonly record struct ColorData(byte R, byte G, byte B, byte A)
{
    public static ColorData FromInt(int color)
    {
        byte ax = (byte)(color & 0xFF);
        byte rx = (byte)((color >> 16) & 0xFF);
        byte gx = (byte)((color >> 8) & 0xFF);
        byte bx = (byte)((color >> 24) & 0xFF);

        return new(rx, gx, bx, ax);
    }
}