

#define isnan _ProtaIsNan

bool _ProtaIsNan(float a) { return !(a < 0.0) && !(a == 0.0) && !(a > 0.0); }
bool _ProtaIsNan(float2 a) { return isnan(a.x) || isnan(a.y); }
bool _ProtaIsNan(float3 a) { return isnan(a.x) || isnan(a.y) || isnan(a.z); }
bool _ProtaIsNan(float4 a) { return isnan(a.x) || isnan(a.y) || isnan(a.z) || isnan(a.w); }


float xmap(float x, float a, float b, float l, float r)
{
    return (x - a) / (b - a) * (r - l) + l;
}

float nan()
{
    return 0.0 / 0.0;
}

void solve2(float a, float b, float c, out float2 root, out int rootCount)
{
    if(abs(b) < 1e-7)
    {
        rootCount = 0;
        root = float2(0, 0);
        return;
    }
    
    if(abs(a) < 1e-7)
    {
        rootCount = 1;
        root = float2(-c / b, -c / b);
        return;
    }
    
    float s = b * b - 4 * a * c;
    if(isnan(s) || s < 0)
    {
        rootCount = 0;
        root = float2(0, 0);
        return;
    }
    
    float ss = sqrt(s);
    rootCount = 2;
    root = float2((-b + ss) / (2 * a), (-b - ss) / (2 * a));
}


float cross(float2 a, float2 b)
{
    return a.x * b.y - a.y * b.x;
}


void RGBtoHSL(float3 color, out float H, out float S, out float L)
{
    float Cmax = max(max(color.r, color.g), color.b);
    float Cmin = min(min(color.r, color.g), color.b);
    L = (Cmax + Cmin) / 2.0;
    S = (Cmax - Cmin) / (1.0 - abs(2.0 * L - 1.0));
    if (Cmax == Cmin)
    {
        H = 0.0;
    }
    else if (Cmax == color.r)
    {
        H = 1/6.0 * ((color.g - color.b) / (Cmax - Cmin)) + 0.0;
    }
    else if (Cmax == color.g)
    {
        H = 1/6.0 * ((color.b - color.r) / (Cmax - Cmin)) + 2/6.0;
    }
    else if (Cmax == color.b)
    {
        H = 1/6.0 * ((color.r - color.g) / (Cmax - Cmin)) + 4/6.0;
    }
    H = fmod(H, 1.0);
}

void RGBtoHSL(float3 color, out float3 hsl)
{
    RGBtoHSL(color, hsl.x, hsl.y, hsl.z);
}

void HSLtoRGB(float H, float S, float L, out float3 color)
{
    H = fmod(H, 1.0);
    S = clamp(S, 0.0, 1.0);
    L = clamp(L, 0.0, 1.0);
    
    float C = (1.0 - abs(2.0 * L - 1.0)) * S;
    float X = C * (1.0 - abs(fmod(H * 6.0, 2.0) - 1.0));
    float m = L - C/2.0;
    if (H < 1.0/6.0)
    {
        color = float3(C + m, X + m, m);
    }
    else if (H < 2.0/6.0)
    {
        color = float3(X + m, C + m, m);
    }
    else if (H < 3.0/6.0)
    {
        color = float3(m, C + m, X + m);
    }
    else if (H < 4.0/6.0)
    {
        color = float3(m, X + m, C + m);
    }
    else if (H < 5.0/6.0)
    {
        color = float3(X + m, m, C + m);
    }
    else if (H <= 1.0)
    {
        color = float3(C + m, m, X + m);
    }
}

void HSLtoRGB(float3 hsl, out float3 color)
{
    HSLtoRGB(hsl.x, hsl.y, hsl.z, color);
}

void HueOffsetHSL(inout float3 hsl, float hueOffset)
{
    hsl.x += hueOffset;
    hsl.x = fmod(hsl.x, 1.0);
}

void HueOffset(inout float4 color, float hueOffset)
{
    float3 hsl;
    RGBtoHSL(color.rgb, hsl);
    HueOffsetHSL(hsl, hueOffset);
    HSLtoRGB(hsl, color.rgb);
}

void SaturationOffsetHSL(inout float3 hsl, float saturationOffset)
{
    if(saturationOffset < 0)
    {
        // 更接近0.
        hsl.y *= 1 + saturationOffset;
    }
    else
    {
        // 更接近1.
        hsl.y = hsl.y + (1.0 - hsl.y) * saturationOffset;
    }
    hsl.y = clamp(hsl.y, 0.0, 1.0);
}

void SaturationOffset(inout float4 color, float saturationOffset)
{
    float3 hsl;
    RGBtoHSL(color.rgb, hsl);
    SaturationOffsetHSL(hsl, saturationOffset);
    hsl.y = clamp(hsl.y, 0.0, 1.0);
    HSLtoRGB(hsl, color.rgb);
}

void BrightnessOffsetHSL(inout float3 hsl, float lightnessOffset)
{
    if(lightnessOffset < 0)
    {
        // 更接近0.
        hsl.z *= 1 + lightnessOffset;
    }
    else
    {
        // 更接近1.
        hsl.z = hsl.z + (1.0 - hsl.z) * lightnessOffset;
    }
}

void BrightnessOffset(inout float4 color, float brightnessOffset)
{
    float3 hsl;
    RGBtoHSL(color.rgb, hsl);
    BrightnessOffsetHSL(hsl, brightnessOffset);
    HSLtoRGB(hsl, color.rgb);
}

void ContrastOffsetHSL(inout float3 hsl, float contrastOffset)
{
    if(contrastOffset < 0)
    {
        // 更接近0.5.
        hsl.y = hsl.y + (0.5 - hsl.y) * -contrastOffset;
    }
    else
    {
        // 更接近0或1.
        hsl.y = hsl.y + (1.0 - hsl.y) * contrastOffset;
        hsl.y = clamp(hsl.y, 0.0, 1.0);
    }
}

void ContrastOffset(inout float4 color, float contrastOffset)
{
    float3 hsl;
    RGBtoHSL(color.rgb, hsl);
    ContrastOffsetHSL(hsl, contrastOffset);
    HSLtoRGB(hsl, color.rgb);
}

void HueConcentrate(inout float4 color, float hue, float hueConcentrate)
{
    float3 hsl;
    RGBtoHSL(color.rgb, hsl);
    
    float hc = hue;
    float hl = hue - 1;
    float hr = hue + 1;
    
    if(hl <= hsl.x && hsl.x <= hc)
    {
        float d1 = hsl.x - hl;
        float d2 = hc - hsl.x;
        if(d1 < d2)
        {
            hsl.x = hl + d1 * (1.0 - hueConcentrate);
        }
        else
        {
            hsl.x = hc - d2 * (1.0 - hueConcentrate);
        }
    }
    else
    {
        float d1 = hsl.x - hc;
        float d2 = hr - hsl.x;
        if(d1 < d2)
        {
            hsl.x = hc + d1 * (1.0 - hueConcentrate);
        }
        else
        {
            hsl.x = hr - d2 * (1.0 - hueConcentrate);
        }
    }
    
    hsl.x = fmod(hsl.x, 1.0);
    
    HSLtoRGB(hsl, color.rgb);
}

