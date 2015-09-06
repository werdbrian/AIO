// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MovementPrediction.cs" company="LeagueSharp">
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
//   The movement prediction.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIO.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.Common;

    using SharpDX;

    using Collision = LeagueSharp.Common.Collision;

    /// <summary>
    ///     The movement prediction.
    /// </summary>
    public static class MovementPrediction
    {
        #region Methods

        /// <summary>
        ///     The get updated prediction.
        /// </summary>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        ///     The <see cref="PredictionOutput" />.
        /// </returns>
        internal static PredictionOutput GetUpdatedPrediction(PredictionInput input)
        {
            if (Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
            {
                input.Speed = 90000;
            }

            var toTarget = Vector3.Normalize(input.Unit.ServerPosition - input.From);
            var targetVelocity = CalculateVelocity(
                input.Unit.ServerPosition, 
                input.Unit.Path.LastOrDefault(), 
                input.Unit.MoveSpeed);

            var a = Vector3.Dot(targetVelocity, targetVelocity) - (input.Speed * input.Speed);
            var b = 2 * Vector3.Dot(targetVelocity, toTarget);
            var c = Vector3.Dot(toTarget, toTarget);

            var p = -b / (2 * a);
            var q = (float)Math.Sqrt((b * b) - 4 * a * c) / (2 * a);

            var theorem1 = p - q;
            var theorem2 = p + q;
            var t = (theorem1 > theorem2 && theorem2 > 0) ? theorem2 : theorem1;

            var result = new PredictionOutput()
                             {
                                 CastPosition = input.Unit.ServerPosition + targetVelocity * (t + input.Delay), 
                                 UnitPosition = input.Unit.ServerPosition, Hitchance = HitChance.VeryHigh
                             };

            // Check if the unit position is in range
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon)
            {
                if (result.Hitchance >= HitChance.High
                    && input.RangeCheckFrom.Distance(input.Unit.Position, true)
                    > Math.Pow(input.Range + input.Radius * 3 / 4, 2))
                {
                    result.Hitchance = HitChance.Medium;
                }

                if (input.RangeCheckFrom.Distance(result.UnitPosition, true)
                    > Math.Pow(input.Range + (input.Type == SkillshotType.SkillshotCircle ? input.Radius : 0), 2))
                {
                    result.Hitchance = HitChance.OutOfRange;
                }
            }

            // Check for collision
            if (input.Collision)
            {
                var positions = new List<Vector3> { result.UnitPosition, result.CastPosition, input.Unit.Position };
                var originalUnit = input.Unit;
                result.CollisionObjects = Collision.GetCollision(positions, input);
                result.CollisionObjects.RemoveAll(x => x.NetworkId == originalUnit.NetworkId);
                result.Hitchance = result.CollisionObjects.Count > 0 ? HitChance.Collision : result.Hitchance;
            }

            return result;
        }

        /// <summary>
        ///     The get updated prediction 2.
        /// </summary>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        ///     The <see cref="PredictionOutput" />.
        /// </returns>
        internal static PredictionOutput GetUpdatedPrediction2(PredictionInput input)
        {
            if (Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
            {
                input.Speed = 90000;
            }

            var targetVelocity = CalculateVelocity(
                input.Unit.ServerPosition, 
                input.Unit.Path.LastOrDefault(), 
                input.Unit.MoveSpeed);
            var position = input.Unit.ServerPosition - input.From;
            var cross = Vector3.Cross(position, targetVelocity);
            var discriminant = input.Speed * input.Speed * position.LengthSquared() - cross.LengthSquared();

            if (discriminant < 0.0f)
            {
                return new PredictionOutput
                           {
                               CastPosition = input.Unit.ServerPosition, UnitPosition = input.Unit.ServerPosition, 
                               Hitchance = HitChance.VeryHigh
                           };
            }

            var time = (Math.Sqrt(discriminant) + Vector3.Dot(position, targetVelocity))
                       / (input.Speed * input.Speed - targetVelocity.LengthSquared());
            var result = new PredictionOutput()
                             {
                                 CastPosition = input.Unit.ServerPosition + targetVelocity * (float)time, 
                                 UnitPosition = input.Unit.ServerPosition, Hitchance = HitChance.VeryHigh
                             };

            // Check if the unit position is in range
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon)
            {
                if (result.Hitchance >= HitChance.High
                    && input.RangeCheckFrom.Distance(input.Unit.Position, true)
                    > Math.Pow(input.Range + input.Radius * 3 / 4, 2))
                {
                    result.Hitchance = HitChance.Medium;
                }

                if (input.RangeCheckFrom.Distance(result.UnitPosition, true)
                    > Math.Pow(input.Range + (input.Type == SkillshotType.SkillshotCircle ? input.Radius : 0), 2))
                {
                    result.Hitchance = HitChance.OutOfRange;
                }
            }

            // Check for collision
            if (input.Collision)
            {
                var positions = new List<Vector3> { result.UnitPosition, result.CastPosition, input.Unit.Position };
                var originalUnit = input.Unit;
                result.CollisionObjects = Collision.GetCollision(positions, input);
                result.CollisionObjects.RemoveAll(x => x.NetworkId == originalUnit.NetworkId);
                result.Hitchance = result.CollisionObjects.Count > 0 ? HitChance.Collision : result.Hitchance;
            }

            return result;
        }

        /// <summary>
        ///     The calculate velocity.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <param name="end">
        ///     The end.
        /// </param>
        /// <param name="speed">
        ///     The speed.
        /// </param>
        /// <returns>
        ///     The <see cref="Vector3" />.
        /// </returns>
        private static Vector3 CalculateVelocity(Vector3 start, Vector3 end, float speed)
        {
            var vector = end - start;
            var d = (float)Math.Sqrt(Vector3.Dot(vector, vector));

            return vector / d * speed;
        }

        #endregion
    }
}