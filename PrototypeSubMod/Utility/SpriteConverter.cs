using UnityEngine;

namespace PrototypeSubMod.Utility;

internal class SpriteConverter
{
    public static Sprite SpriteFromAtlasSprite(Atlas.Sprite atlasSprite)
    {
        Rect rect = new Rect(0, 0, atlasSprite.texture.width, atlasSprite.texture.height);
        Vector2 pivot = new Vector2(atlasSprite.texture.width / 2, atlasSprite.texture.height / 2);
        return Sprite.Create(atlasSprite.texture, rect, pivot);
    }
}
