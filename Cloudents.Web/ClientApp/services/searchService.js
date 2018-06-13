import { search} from "./resources";

export default {
    activateFunction: {
        ask({ source, term=""}) {
            return search.getQuestions({term, source})
        },
        note({ source, university, course, term="", page, sort }) {
            return search.getDocument({ source, university, course, query:term, page, sort });
        },
        flashcard({ source, university, course, term="", page, sort }) {
            return search.getFlashcard({ source, university, course, query:term, page, sort });
        },
        tutor({ term="", filter, sort, page, location }) {
            return search.getTutor({ term, filter, sort, location, page });
        },
        job({ term="", filter, sort, jobType: facet, page, location }) {
            return search.getJob({ term, filter, sort, location, facet, page });
        },
        book({ term="", page }) {
            return search.getBook({ term, page });
        },
        bookDetails({ type, isbn13 }) {
            return search.getBookDetails({ type, isbn13 });
        }
    },
    autoComplete:(term)=>search.autoComplete(term),
    nextPage:(params)=>search.getNextPage(params)
}