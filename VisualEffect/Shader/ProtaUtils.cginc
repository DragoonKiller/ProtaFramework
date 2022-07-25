

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
