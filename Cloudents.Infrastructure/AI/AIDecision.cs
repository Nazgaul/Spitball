using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.AI
{
    public class AIDecision : IDecision
    {
        [Flags]
        private enum Decision
        {
            None = 0,
            Course = 1,
            SearchType = 2,
            Term = 4

        }

        private static readonly Dictionary<Decision, AiResult> NoneIntentMatrix =
            new Dictionary<Decision, AiResult>()
            {
                [Decision.None] = AiResult.SearchOrQuestion,
                [Decision.SearchType] = AiResult.AddSubjectOrCourse,
            };

        private static readonly Dictionary<Decision, AiResult> TutorBookJobMatrix =
            new Dictionary<Decision, AiResult>()
            {
                [Decision.None] = AiResult.AddSubject,
                [Decision.Course] = AiResult.AddSubject,
                [Decision.SearchType] = AiResult.AddSubject,
                [Decision.Course | Decision.SearchType] = AiResult.AddSubject,
            };
        private static readonly Dictionary<Decision, AiResult> AskQuestionMatrix =
            new Dictionary<Decision, AiResult>()
            {
                [Decision.None] = AiResult.SearchOrQuestion,
                [Decision.Course] = AiResult.ChatPost,
                [Decision.SearchType] = AiResult.AddSubjectOrCourse,
                [Decision.Course | Decision.SearchType] = AiResult.AddSearchTypeToSubject,
                [Decision.SearchType | Decision.Term] = AiResult.AddSearchTypeToSubject,
            };

        private static readonly Dictionary<Decision, AiResult> SearchMatrix =
            new Dictionary<Decision, AiResult>()
            {
                [Decision.None] = AiResult.SearchOrQuestion,
                [Decision.Course] = AiResult.SearchOrQuestion,
                [Decision.SearchType] = AiResult.AddSubjectOrCourse,
            };

        private static readonly Dictionary<Decision, AiResult> PurchaseMatrix =
            new Dictionary<Decision, AiResult>()
            {
                [Decision.None] = AiResult.PurchaseAskBuy,
                [Decision.SearchType] = AiResult.PurchaseChangeTerm,
            };

        private readonly Dictionary<AiIntent, Func<Decision, AiResult>> m_Dictionary =
            new Dictionary<AiIntent, Func<Decision, AiResult>>
            {
                [AiIntent.None] = p =>
                {
                    const AiResult result = AiResult.Search;
                    if (NoneIntentMatrix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                },
                [AiIntent.Qna] = _ => AiResult.Qna,
                [AiIntent.Tutor] = p =>
                {
                    const AiResult result = AiResult.Tutor;
                    if (TutorBookJobMatrix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                },
                [AiIntent.Job] = p =>
            {
                const AiResult result = AiResult.Job;
                if (TutorBookJobMatrix.TryGetValue(p, out var result2))
                {
                    return result2;
                }
                return result;
            },
                [AiIntent.Book] = p =>
                {
                    const AiResult result = AiResult.Book;
                    if (TutorBookJobMatrix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                },
                [AiIntent.Question] = p =>
                {
                    const AiResult result = AiResult.Ask;
                    if (AskQuestionMatrix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                },
                [AiIntent.Search] = p =>
                {
                    const AiResult result = AiResult.Search;
                    if (SearchMatrix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                },
                [AiIntent.Purchase] = p =>
                {
                    const AiResult result = AiResult.Purchase;
                    if (PurchaseMatrix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                }
            };

        public (AiResult result, AIDto data) MakeDecision(AIDto aiResult)
        {
            var intentMatrix = m_Dictionary[aiResult.Intent];

            var decision = Decision.None;
            if (aiResult.SearchType != null)
            {
                decision |= Decision.SearchType;
            }
            if (!string.IsNullOrEmpty(aiResult.Course))
            {
                decision |= Decision.Course;
            }
            if (aiResult.Term.Count > 0)
            {
                decision |= Decision.Term;
            }
            var result = intentMatrix(decision);
            if (result == AiResult.AddSearchTypeToSubject && aiResult.SearchType.HasValue)
            {
                aiResult.Term.Add(aiResult.SearchType.Value.Value);
                return (AiResult.Ask, aiResult);
            }
            if (result == AiResult.PurchaseChangeTerm && aiResult.SearchType.HasValue)
            {
                aiResult.Term.Add(aiResult.SearchType.Value.Value);
                return (AiResult.Purchase, aiResult);
            }
            return (result, aiResult);
        }
    }
}
