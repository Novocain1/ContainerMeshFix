using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace ContainerMeshFix
{
    internal static class ProxyMethods
    {
        public static void TesslateShape(ITesselatorAPI tapi, CollectibleObject textureSourceCollectible, Shape shape, out MeshData modeldata, Vec3f meshRotationDeg = null, int? quantityElements = null, string[] selectiveElements = null)
        {
            var game = tapi.GetField<ClientMain>("game");
            var blockAtlasMngr = game.GetField<BlockTextureAtlasManager>("BlockAtlasManager");
            var itemAtlasMngr = game.GetField<ItemTextureAtlasManager>("ItemAtlasManager");
            TextureSource texSource = null;

            string loggedType = "";

            var texloc = new AssetLocation(textureSourceCollectible.Attributes["liquidContainerProps"]?["texture"]?.AsString());

            if (textureSourceCollectible is Block)
            {
                var block = textureSourceCollectible as Block;
                CompositeTexture setTex = null;
                if (texloc != null)
                {
                    setTex = new CompositeTexture(texloc);
                }
                else
                {
                    foreach (var val in block.Textures.Values)
                    {
                        setTex = val;
                        break;
                    }
                }

                (textureSourceCollectible as Block).Textures["obj"] = setTex;
                (textureSourceCollectible as Block).Textures["gltf"] = setTex;

                texSource = new TextureSource(game, blockAtlasMngr.Size, (Block)textureSourceCollectible);
                
                loggedType = "block";
            }
            else if (textureSourceCollectible is Item)
            {
                var item = textureSourceCollectible as Item;
                CompositeTexture setTex = null;
                if (texloc != null)
                {
                    setTex = new CompositeTexture(texloc);
                }
                else
                {
                    foreach (var val in item.Textures.Values)
                    {
                        setTex = val;
                        break;
                    }
                }

                (textureSourceCollectible as Item).Textures["obj"] = setTex;
                (textureSourceCollectible as Item).Textures["gltf"] = setTex;

                texSource = new TextureSource(game, itemAtlasMngr.Size, (Item)textureSourceCollectible);
                loggedType = "item";
            }
            var emptyShapeLoc = new AssetLocation(textureSourceCollectible.Attributes["liquidContainerProps"]["emptyShapeLoc"].AsString());
            emptyShapeLoc = emptyShapeLoc.CloneWithoutPrefixAndEnding(7);
            
            var formatstr = textureSourceCollectible.Attributes["liquidContainerProps"]?["emptyShapeFormat"]?.AsString("VintageStory") ?? "VintageStory";
            Enum.TryParse<EnumShapeFormat>(formatstr, true, out var format);

            var shapeTesselator = tapi as ShapeTesselator;

            var compositeShape = new CompositeShape() { Base = emptyShapeLoc, Format = format };

            //public void TesselateShape(string type, AssetLocation name, CompositeShape compositeShape, out MeshData modeldata, ITexPositionSource texSource, int generalGlowLevel = 0, byte climateColorMapIndex = 0, byte seasonColorMapIndex = 0, int? quantityElements = null, string[] selectiveElements = null)
            shapeTesselator
                .TesselateShape(
                loggedType, 
                textureSourceCollectible.Code, 
                compositeShape, 
                out modeldata, 
                texSource, 0, 0, 0, quantityElements, selectiveElements);

            modeldata = modeldata.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), meshRotationDeg.X * GameMath.DEG2RAD, meshRotationDeg.Y * GameMath.DEG2RAD, meshRotationDeg.Z * GameMath.DEG2RAD);
        }
    }
}
