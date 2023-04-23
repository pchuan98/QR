using System;
using System.Collections.Generic;

namespace QR.Core.Helpers.PDF;

/// <summary>
/// 四个Grid的内容全部都有小方格
/// </summary>
public class PureSpaceView : BaseQuarterDocumentView
{
    public int SpaceCount { get; set; }

    public List<bool>? IsSpaceCollection { get; set; } = null;

    public override List<TableModel> CreateTableModels()
    {
        List<TableModel> tables = new();

        int length = this.TableRows * this.TableColumns;
        int count = (int)Math.Ceiling((double)this.SourceData.Count / (double)length);

        for (int i = 1; i <= count; i++)
        {
            // 实际创建当前table使用的SpaceCount
            var scount = SpaceCount;

            if (IsSpaceCollection != null)
            {
                int index = (i - 1) % IsSpaceCollection.Count;
                scount = IsSpaceCollection[index] ? SpaceCount : 0;
            }


            var data = Helpers.ListHelper<string>.Split(this.SourceData, length, i);
            var spaceData = Helpers.ListHelper<string>.Join(data, "", scount);


            tables.Add(new()
            {
                CalculateTableWidths = (c, r, w, h)
                    => CalculateHelper.BoxSizeWidth(scount, this.TableColumns, c, r, w, h),
                Data = spaceData.ToArray(),
                ColumnDefinitions = this.TableColumns * (scount + 1),
                RowDefinitions = this.TableRows,
                Border = this.TableBorder,
                TableDefaultFontFamily = this.FontFamily,
                TableDefaultFontSize = this.FontSize,
            });
        }
        return tables;
    }
}