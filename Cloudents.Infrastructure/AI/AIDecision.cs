using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.AI
{
    public class AIDecision : IDesicions
    {
        [Flags]
        private enum Decision
        {
            None = 0,
            Course = 1,
            SearchType = 2,
            Term = 4

        }

        private static readonly Dictionary<Decision, AIResult> NoneIntentMatix =
            new Dictionary<Decision, AIResult>()
            {
                [Decision.None] = AIResult.SearchOrQuestion,
                [Decision.SearchType] = AIResult.AddSubjectOrCourse,
            };

        private static readonly Dictionary<Decision, AIResult> TutorBookJobMatix =
            new Dictionary<Decision, AIResult>()
            {
                [Decision.None] = AIResult.AddSubject,
                [Decision.Course] = AIResult.AddSubject,
                [Decision.SearchType] = AIResult.AddSubject,
                [Decision.Course | Decision.SearchType] = AIResult.AddSubject,
            };
        private static readonly Dictionary<Decision, AIResult> AskQuestionMatix =
            new Dictionary<Decision, AIResult>()
            {
                [Decision.None] = AIResult.SearchOrQuestion,
                [Decision.Course] = AIResult.ChatPost,
                [Decision.SearchType] = AIResult.AddSubjectOrCourse,
                [Decision.Course | Decision.SearchType] = AIResult.AddSearchTypeToSubject,
                [Decision.SearchType | Decision.Term] = AIResult.AddSearchTypeToSubject,
            };

        private static readonly Dictionary<Decision, AIResult> SearchMatrix =
            new Dictionary<Decision, AIResult>()
            {
                [Decision.None] = AIResult.SearchOrQuestion,
                [Decision.Course] = AIResult.SearchOrQuestion,
                [Decision.SearchType] = AIResult.AddSubjectOrCourse,
            };

        private static readonly Dictionary<Decision, AIResult> PurcahseMatrix =
            new Dictionary<Decision, AIResult>()
            {
                [Decision.None] = AIResult.PurchaseAskBuy,
                [Decision.SearchType] = AIResult.PurchaseChangeTerm,
            };

        private readonly Dictionary<AIIntent, Func<Decision, AIResult>> m_Dictionary =
            new Dictionary<AIIntent, Func<Decision, AIResult>>
            {
                [AIIntent.None] = p =>
                {
                    const AIResult result = AIResult.Search;
                    if (NoneIntentMatix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                },
                [AIIntent.Qna] = _ => AIResult.Qna,
                [AIIntent.Tutor] = p =>
                {
                    const AIResult result = AIResult.Tutor;
                    if (TutorBookJobMatix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                },
                [AIIntent.Job] = p =>
            {
                const AIResult result = AIResult.Jobs;
                if (TutorBookJobMatix.TryGetValue(p, out var result2))
                {
                    return result2;
                }
                return result;
            },
                [AIIntent.Book] = p =>
                {
                    const AIResult result = AIResult.Books;
                    if (TutorBookJobMatix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                },
                [AIIntent.Question] = p =>
                {
                    const AIResult result = AIResult.Question;
                    if (AskQuestionMatix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                },
                [AIIntent.Search] = p =>
                {
                    const AIResult result = AIResult.Search;
                    if (SearchMatrix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                },
                [AIIntent.Purchase] = p =>
                {
                    const AIResult result = AIResult.Purchase;
                    if (PurcahseMatrix.TryGetValue(p, out var result2))
                    {
                        return result2;
                    }
                    return result;
                }
            };

        public (AIResult result, AIDto data) MakeDesicision(AIDto aiResult)
        {
            var intentMatix = m_Dictionary[aiResult.Intent];

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
            var result = intentMatix(decision);
            if (result == AIResult.AddSearchTypeToSubject && aiResult.SearchType.HasValue)
            {
                aiResult.Term.Add(aiResult.SearchType.Value.Value);
                return (AIResult.Question, aiResult);
            }
            if (result == AIResult.PurchaseChangeTerm && aiResult.SearchType.HasValue)
            {
                aiResult.Term.Add(aiResult.SearchType.Value.Value);
                return (AIResult.Purchase, aiResult);
            }
            return (result, aiResult);
        }
    }
}
