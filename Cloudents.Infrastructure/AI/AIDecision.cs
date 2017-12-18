using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.AI
{
    public class AIDecision : IDecision
    {
        [AttributeUsage(AttributeTargets.Method)]
        private sealed class FactoryAttribute : Attribute
        {

        }

        private readonly List<Func<AiDto, VerticalEngineDto>> _matrix = new List<Func<AiDto, VerticalEngineDto>>();



        public AIDecision()
        {
            var methods = typeof(AIDecision).GetMethods(BindingFlags.NonPublic |BindingFlags.Static).Where(mi =>
            {
                if (mi.ReturnType != typeof(VerticalEngineDto))
                {
                    return false;
                }
                var t = mi.GetCustomAttributes(typeof(FactoryAttribute), true);
                return t.Length != 0;
            });
            foreach (var method in methods)
            {
                var t = (Func<AiDto, VerticalEngineDto>)Delegate.CreateDelegate(typeof(Func<AiDto, VerticalEngineDto>), method);
                _matrix.Add(t);
            }
        }

        public VerticalEngineDto MakeDecision(AiDto aiResult)
        {
            foreach (var vertical in _matrix)
            {
                var result = vertical(aiResult);
                if (result != null)
                {
                    return result;
                }
            }
            return new VerticalEngineNoneDto();

        }

        [Factory]
        private static VerticalEngineDto DocumentEngine(AiDto aiResult)
        {
            if (aiResult.Intent != AiIntent.Search)
            {
                return null;
            }
            if (aiResult.SearchType.HasValue && string.Equals(aiResult.SearchType.Value.Key, Flashcards,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            var terms = aiResult.Subject;
            terms.AddNotNull(aiResult.Course);
            terms.AddNotNull(aiResult.Location);

            if (terms.Count == 0)
            {
                return null;
            }
            return new VerticalEngineSearchDto(terms, aiResult.University, aiResult.SearchType?.Value);
        }

        private const string Flashcards = "flashcards";

        [Factory]
        private static VerticalEngineDto FlashcardEngine(AiDto aiResult)
        {
            if (aiResult.Intent != AiIntent.Search)
            {
                return null;
            }
            if (!aiResult.SearchType.HasValue || !string.Equals(aiResult.SearchType.Value.Key, Flashcards,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            var terms = aiResult.Subject;
            terms.AddNotNull(aiResult.Course);
            terms.AddNotNull(aiResult.Location);

            if (terms.Count == 0)
            {
                return null;
            }
            return new VerticalEngineFlashcardDto(terms, aiResult.University);
        }

        [Factory]

        private static VerticalEngineDto AskEngine(AiDto aiResult)
        {
            if (aiResult.Intent != AiIntent.None && aiResult.Intent != AiIntent.Question) return null;
            var terms = aiResult.Subject;
            terms.AddNotNull(aiResult.Location);

            if (terms.Count == 0)
            {
                return null;
            }
            return new VerticalEngineAskDto(terms);
        }

        [Factory]

        private static VerticalEngineDto JobEngine(AiDto aiResult)
        {
            if (aiResult.Intent != AiIntent.Job)
            {
                return null;
            }
            return new VerticalEngineJobDto(aiResult.Subject, aiResult.Location);
        }

        [Factory]

        private static VerticalEngineDto TutorEngine(AiDto aiResult)
        {
            if (aiResult.Intent != AiIntent.Tutor)
            {
                return null;
            }
            if (aiResult.Subject.Count == 0)
            {
                return null;
            }
            return new VerticalEngineTutorDto(aiResult.Subject, aiResult.Location);
        }

        [Factory]

        private static VerticalEngineDto FoodEngine(AiDto aiResult)
        {
            if (aiResult.Intent != AiIntent.Purchase)
            {
                return null;
            }
            if (aiResult.Subject.Count == 0)
            {
                return null;
            }
            return new VerticalEngineFoodDto(aiResult.Subject, aiResult.Location);
        }

        [Factory]
        private static VerticalEngineDto BookEngine(AiDto aiResult)
        {

            if (aiResult.Intent != AiIntent.Book)
            {
                return null;
            }
            var terms = aiResult.Subject;
            terms.AddNotNull(aiResult.Isbn);
            if (terms.Count == 0)
            {
                return null;
            }
            return new VerticalEngineBookDto(terms);
        }
    }
}
