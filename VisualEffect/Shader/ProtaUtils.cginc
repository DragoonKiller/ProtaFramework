

float xmap(float x, float a, float b, float l, float r)
{
    return (x - a) / (b - a) * (r - l) + l;
}

bool isNan(float x)
{
    return !(x < 0.f || x > 0.f || x == 0.f);
}

float nan()
{
    return 0.0 / 0.0;
}

float2 solve2(float a, float b, float c)
{
    if(abs(a) < 1e-6) return -c / b;
    float s = b * b - 4 * a * c;
    if(isNan(s) || s < 0) return float2(nan(), nan());
    float ss = sqrt(s);
    return float2((-b + ss) / (2 * a), (-b - ss) / (2 * a));
}


float cross(float2 a, float2 b)
{
    return a.x * b.y - a.y * b.x;
}
