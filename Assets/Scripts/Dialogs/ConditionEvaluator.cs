using System;
using UnityEngine;

namespace Dialogs
{
    /// <summary>
    /// Система оценки условий для диалогов
    /// </summary>
    public static class ConditionEvaluator
    {
        /// <summary>
        /// Проверить условие
        /// </summary>
        public static bool Evaluate(Condition condition)
        {
            if (condition == null) return true;

            try
            {
                return condition.Evaluate();
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка при оценке условия: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Проверить все условия в списке
        /// </summary>
        public static bool EvaluateAll(System.Collections.Generic.List<Condition> conditions)
        {
            if (conditions == null || conditions.Count == 0) return true;

            foreach (var condition in conditions)
            {
                if (!Evaluate(condition)) return false;
            }

            return true;
        }

        /// <summary>
        /// Проверить хотя бы одно условие из списка
        /// </summary>
        public static bool EvaluateAny(System.Collections.Generic.List<Condition> conditions)
        {
            if (conditions == null || conditions.Count == 0) return true;

            foreach (var condition in conditions)
            {
                if (Evaluate(condition)) return true;
            }

            return false;
        }
    }
}
