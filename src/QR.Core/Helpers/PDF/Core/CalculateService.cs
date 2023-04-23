namespace QR.Core.Helpers.PDF;

/// <summary>
/// 计算table单个cell尺寸
/// </summary>
/// <param name="cd">Columns Definition Count</param>
/// <param name="rd">Rows Definition Count</param>
/// <param name="width">Table Width</param>
/// <param name="height">Table Height</param>
/// <returns></returns>
public delegate float[] CalculateTableSize(int cd, int rd, float width, float height);