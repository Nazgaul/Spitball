import { search} from "./resources";

export default {
    activateFunction: {
        ask({ source, university, course, term="", page, sort, q: userText }) {
            return search.getQna({ source, university, course, term, page, sort, userText });
        },
        note({ source, university, course, term="", page, sort, docType }) {
            return search.getDocument({ source, university, course, query:term, page, sort, docType });
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
        },
        foodDetails({ id }) {
            return search.getPlacesDetails({ id });
        },
        food({ term="", filter,  location }) {
                return search.getFood({ term, filter, location });

        }
    },
    autoComplete:(term)=>search.autoComplete(term),
    nextPage:(params)=>search.getNextPage(params)
}