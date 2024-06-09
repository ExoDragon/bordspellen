using Core.Domain.Data.Entities;

namespace BordSpellen.Util;

public static class BoardGameHelper
{
    public static string ImageStringFromByteArray(this BoardGame boardGame)
    {
        if (boardGame.Image == null || String.IsNullOrEmpty(boardGame.ImageFormat)) return "";

        var image = Convert.ToBase64String(boardGame.Image);
        var type = boardGame.ImageFormat;
        return $"data:{type};base64,{image}";
    }
}