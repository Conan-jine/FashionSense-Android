﻿using HarmonyLib;
using LivelyHair.Framework.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace LivelyHair.Framework.Patches.Entities
{
    internal class FarmerRendererPatch : PatchTemplate
    {
        private readonly Type _entity = typeof(FarmerRenderer);

        internal FarmerRendererPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(FarmerRenderer.drawHairAndAccesories), new[] { typeof(SpriteBatch), typeof(int), typeof(Farmer), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(float), typeof(Color), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawHairAndAccesoriesPrefix)));

            harmony.CreateReversePatcher(AccessTools.Method(_entity, "executeRecolorActions", new[] { typeof(Farmer) }), new HarmonyMethod(GetType(), nameof(ExecuteRecolorActionsReversePatch))).Patch();
        }

        private static void DrawShirt(SpriteBatch b, Rectangle shirtSourceRect, Rectangle dyed_shirt_source_rect, FarmerRenderer renderer, Farmer who, int currentFrame, float rotation, float scale, float layerDepth, Vector2 position, Vector2 origin, Vector2 positionOffset, Color overrideColor)
        {
            float dye_layer_offset = 1E-07f;

            if (!who.bathingClothes)
            {
                b.Draw(FarmerRenderer.shirtsTexture, position + origin + positionOffset + new Vector2(16 + FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, (float)(56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)renderer.heightOffset * scale - (float)(who.IsMale ? 0 : 0)), shirtSourceRect, overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.5E-07f);
                b.Draw(FarmerRenderer.shirtsTexture, position + origin + positionOffset + new Vector2(16 + FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, (float)(56 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)renderer.heightOffset * scale - (float)(who.IsMale ? 0 : 0)), dyed_shirt_source_rect, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.5E-07f + dye_layer_offset);
            }
        }

        private static void DrawAccessory(SpriteBatch b, Rectangle accessorySourceRect, FarmerRenderer renderer, Farmer who, int currentFrame, float rotation, float scale, float layerDepth, Vector2 position, Vector2 origin, Vector2 positionOffset, Vector2 rotationAdjustment, Color overrideColor)
        {
            if ((int)who.accessory >= 0)
            {
                b.Draw(FarmerRenderer.accessoriesTexture, position + origin + positionOffset + rotationAdjustment + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 8 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)renderer.heightOffset - 4), accessorySourceRect, (overrideColor.Equals(Color.White) && (int)who.accessory < 6) ? ((Color)who.hairstyleColor) : overrideColor, rotation, origin, 4f * scale + ((rotation != 0f) ? 0f : 0f), SpriteEffects.None, layerDepth + (((int)who.accessory < 8) ? 1.9E-05f : 2.9E-05f));
            }
        }

        private static void DrawHat(SpriteBatch b, Rectangle hatSourceRect, FarmerRenderer renderer, Farmer who, int currentFrame, float rotation, float scale, float layerDepth, Vector2 position, Vector2 origin, Vector2 positionOffset)
        {
            if (who.hat.Value != null && !who.bathingClothes)
            {
                bool flip = who.FarmerSprite.CurrentAnimationFrame.flip;
                float layer_offset = 3.9E-05f;
                if (who.hat.Value.isMask && who.facingDirection == 0)
                {
                    Rectangle mask_draw_rect = hatSourceRect;
                    mask_draw_rect.Height -= 11;
                    mask_draw_rect.Y += 11;
                    b.Draw(FarmerRenderer.hatsTexture, position + origin + positionOffset + new Vector2(0f, 44f) + new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((!who.hat.Value.ignoreHairstyleOffset) ? FarmerRenderer.hairstyleHatOffset[(int)who.hair % 16] : 0) + 4 + (int)renderer.heightOffset), mask_draw_rect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + layer_offset);
                    mask_draw_rect = hatSourceRect;
                    mask_draw_rect.Height = 11;
                    layer_offset = -1E-06f;
                    b.Draw(FarmerRenderer.hatsTexture, position + origin + positionOffset + new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((!who.hat.Value.ignoreHairstyleOffset) ? FarmerRenderer.hairstyleHatOffset[(int)who.hair % 16] : 0) + 4 + (int)renderer.heightOffset), mask_draw_rect, who.hat.Value.isPrismatic ? Utility.GetPrismaticColor() : Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + layer_offset);
                }
                else
                {
                    b.Draw(FarmerRenderer.hatsTexture, position + origin + positionOffset + new Vector2(-8 + ((!flip) ? 1 : (-1)) * FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, -16 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((!who.hat.Value.ignoreHairstyleOffset) ? FarmerRenderer.hairstyleHatOffset[(int)who.hair % 16] : 0) + 4 + (int)renderer.heightOffset), hatSourceRect, who.hat.Value.isPrismatic ? Utility.GetPrismaticColor() : Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + layer_offset);
                }
            }
        }

        private static bool DrawHairAndAccesoriesPrefix(FarmerRenderer __instance, Vector2 ___positionOffset, Vector2 ___rotationAdjustment, ref Rectangle ___shirtSourceRect, ref Rectangle ___accessorySourceRect, ref Rectangle ___hatSourceRect, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth)
        {
            if (!who.modData.ContainsKey("LivelyHair.CustomHair.Id"))
            {
                return true;
            }

            var appearanceModel = LivelyHair.textureManager.GetSpecificAppearanceModel(who.modData["LivelyHair.CustomHair.Id"]);
            if (appearanceModel is null)
            {
                return true;
            }

            HairModel hairModel = null;
            switch (who.FacingDirection)
            {
                case 0:
                    hairModel = appearanceModel.BackHair;
                    break;
                case 1:
                    hairModel = appearanceModel.RightHair;
                    break;
                case 2:
                    hairModel = appearanceModel.FrontHair;
                    break;
                case 3:
                    hairModel = appearanceModel.LeftHair;
                    break;
            }

            if (hairModel is null)
            {
                return true;
            }

            Rectangle sourceRect = new Rectangle(hairModel.StartingPosition.X, hairModel.StartingPosition.Y, hairModel.HairSize.Width, hairModel.HairSize.Length);
            // Handle any animation offsets
            if (hairModel.HasIdleAnimation())
            {
                if (!who.modData.ContainsKey("LivelyHair.Animation.FrameIndex") || !who.modData.ContainsKey("LivelyHair.Animation.FrameDuration") || !who.modData.ContainsKey("LivelyHair.Animation.ElapsedDuration"))
                {
                    who.modData["LivelyHair.Animation.FrameIndex"] = "0";
                    who.modData["LivelyHair.Animation.FrameDuration"] = hairModel.GetIdleAnimationAtFrame(0).Duration.ToString();
                    who.modData["LivelyHair.Animation.ElapsedDuration"] = "0";
                }

                var frameIndex = Int32.Parse(who.modData["LivelyHair.Animation.FrameIndex"]);
                var frameDuration = Int32.Parse(who.modData["LivelyHair.Animation.FrameDuration"]);
                var elapsedDuration = Int32.Parse(who.modData["LivelyHair.Animation.ElapsedDuration"]);

                if (elapsedDuration >= frameDuration)
                {
                    frameIndex = frameIndex + 1 >= hairModel.IdleAnimation.Count() ? 0 : frameIndex + 1;

                    who.modData["LivelyHair.Animation.FrameIndex"] = frameIndex.ToString();
                    who.modData["LivelyHair.Animation.FrameDuration"] = hairModel.GetIdleAnimationAtFrame(frameIndex).Duration.ToString();
                    who.modData["LivelyHair.Animation.ElapsedDuration"] = "0";
                }
                else
                {
                    who.modData["LivelyHair.Animation.ElapsedDuration"] = (elapsedDuration + Game1.currentGameTime.ElapsedGameTime.Milliseconds).ToString();
                }

                sourceRect.X += sourceRect.Width * frameIndex;
            }

            int hair_style = who.getHair();
            HairStyleMetadata hair_metadata = Farmer.GetHairStyleMetadata(who.hair.Value);
            if (who != null && who.hat.Value != null && who.hat.Value.hairDrawType.Value == 1 && hair_metadata != null && hair_metadata.coveredIndex != -1)
            {
                hair_style = hair_metadata.coveredIndex;
                hair_metadata = Farmer.GetHairStyleMetadata(hair_style);
            }

            // Execute recolor
            ExecuteRecolorActionsReversePatch(__instance, who);

            // Set the source rectangles for shirts, accessories and hats
            ___shirtSourceRect = new Rectangle(__instance.ClampShirt(who.GetShirtIndex()) * 8 % 128, __instance.ClampShirt(who.GetShirtIndex()) * 8 / 128 * 32, 8, 8);
            if ((int)who.accessory >= 0)
            {
                ___accessorySourceRect = new Rectangle((int)who.accessory * 16 % FarmerRenderer.accessoriesTexture.Width, (int)who.accessory * 16 / FarmerRenderer.accessoriesTexture.Width * 32, 16, 16);
            }
            if (who.hat.Value != null)
            {
                ___hatSourceRect = new Rectangle(20 * (int)who.hat.Value.which % FarmerRenderer.hatsTexture.Width, 20 * (int)who.hat.Value.which / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20);
            }

            Rectangle dyed_shirt_source_rect = ___shirtSourceRect;
            dyed_shirt_source_rect = ___shirtSourceRect;
            dyed_shirt_source_rect.Offset(128, 0);

            // Offset the source rectangles for shirts, accessories and hats according to facingDirection
            OffsetSourceRectangles(who, rotation, ref ___shirtSourceRect, ref dyed_shirt_source_rect, ref ___accessorySourceRect, ref ___hatSourceRect, ref ___rotationAdjustment);

            // Draw the shirt and accessory
            DrawShirt(b, ___shirtSourceRect, dyed_shirt_source_rect, __instance, who, currentFrame, rotation, scale, layerDepth, position, origin, ___positionOffset, overrideColor);
            DrawAccessory(b, ___accessorySourceRect, __instance, who, currentFrame, rotation, scale, layerDepth, position, origin, ___positionOffset, ___rotationAdjustment, overrideColor);

            // Draw hair
            float hair_draw_layer = 2.2E-05f;
            b.Draw(appearanceModel.Texture, position + origin + ___positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair >= 16) ? (-4) : ((!who.IsMale && (int)who.hair < 16) ? 4 : 0))), sourceRect, overrideColor.Equals(Color.White) ? ((Color)who.hairstyleColor) : overrideColor, rotation, origin + new Vector2(hairModel.HeadPosition.X, 16), 4f * scale, hairModel.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + hair_draw_layer);

            // Perform hat draw logic
            DrawHat(b, ___hatSourceRect, __instance, who, currentFrame, rotation, scale, layerDepth, position, origin, ___positionOffset);
            return false;
        }

        private static void OffsetSourceRectangles(Farmer who, float rotation, ref Rectangle shirtSourceRect, ref Rectangle dyed_shirt_source_rect, ref Rectangle accessorySourceRect, ref Rectangle hatSourceRect, ref Vector2 rotationAdjustment)
        {
            switch (who.FacingDirection)
            {
                case 0:
                    shirtSourceRect.Offset(0, 24);
                    //hairstyleSourceRect.Offset(0, 64);

                    dyed_shirt_source_rect = shirtSourceRect;
                    dyed_shirt_source_rect.Offset(128, 0);
                    if (who.hat.Value != null)
                    {
                        hatSourceRect.Offset(0, 60);
                    }

                    return;
                case 1:
                    shirtSourceRect.Offset(0, 8);
                    //hairstyleSourceRect.Offset(0, 32);
                    dyed_shirt_source_rect = shirtSourceRect;
                    dyed_shirt_source_rect.Offset(128, 0);

                    if ((int)who.accessory >= 0)
                    {
                        accessorySourceRect.Offset(0, 16);
                    }
                    if (who.hat.Value != null)
                    {
                        hatSourceRect.Offset(0, 20);
                    }
                    if (rotation == -(float)Math.PI / 32f)
                    {
                        rotationAdjustment.X = 6f;
                        rotationAdjustment.Y = -2f;
                    }
                    else if (rotation == (float)Math.PI / 32f)
                    {
                        rotationAdjustment.X = -6f;
                        rotationAdjustment.Y = 1f;
                    }

                    return;
                case 2:
                    dyed_shirt_source_rect = shirtSourceRect;
                    dyed_shirt_source_rect.Offset(128, 0);

                    return;
                case 3:
                    {
                        bool flip2 = true;
                        shirtSourceRect.Offset(0, 16);
                        dyed_shirt_source_rect = shirtSourceRect;
                        dyed_shirt_source_rect.Offset(128, 0);

                        if ((int)who.accessory >= 0)
                        {
                            accessorySourceRect.Offset(0, 16);
                        }

                        /*
                        if (hair_metadata != null && hair_metadata.usesUniqueLeftSprite)
                        {
                            flip2 = false;
                            hairstyleSourceRect.Offset(0, 96);
                        }
                        else
                        {
                            hairstyleSourceRect.Offset(0, 32);
                        }
                        */

                        if (who.hat.Value != null)
                        {
                            hatSourceRect.Offset(0, 40);
                        }
                        if (rotation == -(float)Math.PI / 32f)
                        {
                            rotationAdjustment.X = 6f;
                            rotationAdjustment.Y = -2f;
                        }
                        else if (rotation == (float)Math.PI / 32f)
                        {
                            rotationAdjustment.X = -5f;
                            rotationAdjustment.Y = 1f;
                        }

                        return;
                    }
            }
        }

        private static void ExecuteRecolorActionsReversePatch(FarmerRenderer __instance, Farmer who)
        {
            new NotImplementedException("It's a stub!");
        }
    }
}
