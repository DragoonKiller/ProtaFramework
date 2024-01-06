

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

void HSLToRGB(float H, float S, float L, out float3 color)
{
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


float4 HueOffset(float4 color, float hueOffset)
{
    float3 hsl;
    RGBtoHSL(color.rgb, hsl.x, hsl.y, hsl.z);
    hsl.x += hueOffset;
    hsl.x = fmod(hsl.x, 1.0);
    HSLToRGB(hsl.x, hsl.y, hsl.z, color.rgb);
    return color;
}

float4 SaturationOffset(float4 color, float saturationOffset)
{
    float3 hsl;
    RGBtoHSL(color.rgb, hsl.x, hsl.y, hsl.z);
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
    HSLToRGB(hsl.x, hsl.y, hsl.z, color.rgb);
    return color;
}

float4 BrightnessOffset(float4 color, float brightnessOffset)
{
    float3 hsl;
    RGBtoHSL(color.rgb, hsl.x, hsl.y, hsl.z);
    if(brightnessOffset < 0)
    {
        // 更接近0.
        hsl.z *= 1 + brightnessOffset;
    }
    else
    {
        // 更接近1.
        hsl.z = hsl.z + (1.0 - hsl.z) * brightnessOffset;
    }
    hsl.z = clamp(hsl.z, 0.0, 1.0);
    HSLToRGB(hsl.x, hsl.y, hsl.z, color.rgb);
    return color;
}

float4 ContrastOffset(float4 color, float contrastOffset)
{
    float3 hsl;
    RGBtoHSL(color.rgb, hsl.x, hsl.y, hsl.z);
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
    HSLToRGB(hsl.x, hsl.y, hsl.z, color.rgb);
    return color;
}

