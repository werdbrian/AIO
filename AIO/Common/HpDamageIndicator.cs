// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpDamageIndicator.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by 
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//   
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
//    GNU General Public License for more details.
//   
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The hp bar indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     The health point bar indicator.
    /// </summary>
    public sealed class HpBarIndicator
    {
        #region Constants

        /// <summary>
        ///     The height.
        /// </summary>
        private const float Height = 9;

        /// <summary>
        ///     The width.
        /// </summary>
        private const float Width = 104;

        #endregion

        #region Fields

        /// <summary>
        ///     The base.
        /// </summary>
        private readonly Obj_AI_Hero @base;

        /// <summary>
        ///     The line.
        /// </summary>
        private readonly Line line;

        /// <summary>
        ///     The spells.
        /// </summary>
        private readonly Dictionary<SpellSlot, bool> spells = new Dictionary<SpellSlot, bool>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HpBarIndicator" /> class.
        /// </summary>
        /// <param name="base">
        ///     The base.
        /// </param>
        public HpBarIndicator(Obj_AI_Hero @base)
        {
            this.@base = @base;
            this.Device = Drawing.Direct3DDevice;
            this.line = new Line(this.Device) { Width = 9 };
            this.DrawColor = new Color(Color.Green.ToVector3(), 40);
            this.Enabled = true;

            Drawing.OnPreReset += this.Drawing_OnPreReset;
            Drawing.OnPostReset += this.Drawing_OnPostReset;
            Drawing.OnEndScene += this.Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload += this.CurrentDomain_DomainUnload;
            AppDomain.CurrentDomain.ProcessExit += this.CurrentDomain_ProcessExit;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the draw color.
        /// </summary>
        public Color DrawColor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public Obj_AI_Hero Target { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the device.
        /// </summary>
        private Device Device { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add spell.
        /// </summary>
        /// <param name="slot">
        ///     The slot.
        /// </param>
        /// <param name="enabled">
        ///     The enabled.
        /// </param>
        public void AddSpell(SpellSlot slot, bool enabled = true)
        {
            this.spells.Add(slot, enabled);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The calculate damage.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="spells">
        ///     The spells.
        /// </param>
        /// <returns>
        /// </returns>
        private static IEnumerable<float> CalculateDamage(
            Obj_AI_Hero source, 
            Obj_AI_Base target, 
            IEnumerable<SpellSlot> spells)
        {
            return spells.Where(s => s.IsReady()).Select(spell => (float)CalculateDamage(source, target, spell));
        }

        /// <summary>
        ///     The calculate damage.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="spell">
        ///     The spell.
        /// </param>
        /// <returns>
        /// </returns>
        private static double CalculateDamage(Obj_AI_Hero source, Obj_AI_Base target, SpellSlot spell)
        {
            return source.GetSpellDamage(target, spell);
        }

        /// <summary>
        ///     The calculate offset.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <returns>
        /// </returns>
        private static Vector2 CalculateOffset(Obj_AI_Base unit)
        {
            return unit.IsAlly ? new Vector2(34, 9) : new Vector2(10, 20);
        }

        /// <summary>
        ///     The calculate percent.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <param name="damage">
        ///     The damage.
        /// </param>
        /// <returns>
        /// </returns>
        private static double CalculatePercent(Obj_AI_Base unit, float damage)
        {
            return Math.Max(unit.Health - damage, 0) / unit.MaxHealth;
        }

        /// <summary>
        ///     The get start position.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <returns>
        /// </returns>
        private static Vector2 GetStartPosition(Obj_AI_Base unit)
        {
            return unit.HPBarPosition + CalculateOffset(unit);
        }

        /// <summary>
        ///     The calculate position after.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <param name="damage">
        ///     The damage.
        /// </param>
        /// <returns>
        /// </returns>
        private Vector2 CalculatePositionAfter(Obj_AI_Base unit, float damage)
        {
            var vector = GetStartPosition(unit);
            return new Vector2(vector.X + ((float)CalculatePercent(unit, damage) * Width), vector.Y);
        }

        /// <summary>
        ///     The current domain_ domain unload.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            this.line.Dispose();
        }

        /// <summary>
        ///     The current domain_ process exit.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            this.line.Dispose();
        }

        /// <summary>
        ///     The drawing_ on end scene.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void Drawing_OnEndScene(EventArgs args)
        {
            if (this.Target == null || !this.Enabled)
            {
                return;
            }

            var enabledSpells = this.spells.Where(entry => entry.Key.IsReady() && entry.Value)
                .Select(entry => entry.Key);
            var start = this.CalculatePositionAfter(this.Target, 0);
            var end = this.CalculatePositionAfter(
                this.Target, 
                CalculateDamage(this.@base, this.Target, enabledSpells.ToArray()).Sum());

            this.Fill(start, end, this.DrawColor);
        }

        /// <summary>
        ///     The drawing_ on post reset.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void Drawing_OnPostReset(EventArgs args)
        {
            this.line.OnResetDevice();
        }

        /// <summary>
        ///     The drawing_ on pre reset.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void Drawing_OnPreReset(EventArgs args)
        {
            this.line.OnLostDevice();
        }

        /// <summary>
        ///     The fill.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <param name="end">
        ///     The end.
        /// </param>
        /// <param name="color">
        ///     The color.
        /// </param>
        private void Fill(Vector2 start, Vector2 end, Color color)
        {
            this.line.Begin();
            this.line.Draw(
                new[] { new Vector2(start.X, start.Y + 3f), new Vector2(end.X, end.Y + 3f) }, 
                new ColorBGRA(color.R, color.G, color.B, color.A));
            this.line.End();
        }

        #endregion
    }
}