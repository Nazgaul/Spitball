import { search, flashcard } from "./resources";
import axios from "axios";
import qs from "qs"
axios.defaults.paramsSerializer=params => qs.stringify(params, { indices: false });
axios.defaults.baseURL="api/";
const previewMap = {
    item(id) {
        return {
            //blob: ["https://zboxstorage.blob.core.windows.net/zboxcahce/850de13a-001a-4284-9fa8-68d97dcd81adV6_0_.docx.svg?sv=2016-05-31&sr=b&sig=3Ymn%2BX5WFN%2BuBA3kaxImosiQQhdSeK0QmZNcItQQUX8%3D&st=2017-11-07T09%3A19%3A47Z&se=2017-11-07T09%3A40%3A47Z&sp=r", "https://zboxstorage.blob.core.windows.net/zboxcahce/850de13a-001a-4284-9fa8-68d97dcd81adV6_1_.docx.svg?sv=2016-05-31&sr=b&sig=zhbfiY%2Fi6Z%2F6i2995OevC1CXsJU9jTx7eaekzl8m9pA%3D&st=2017-11-07T09%3A19%3A47Z&se=2017-11-07T09%3A40%3A47Z&sp=r"]
            //            blob: ["https://zboxstorage.blob.core.windows.net/zboxcahce/c0a06374-acc2-4815-89cc-8f5bb11cda4cV3_.cpp.html?sv=2016-05-31&sr=b&sig=4EowLDbARoM5%2F3CiSJe1YQwHVhnKEdbn%2BadeIuE%2BsPc%3D&st=2017-11-07T09%3A24%3A23Z&se=2017-11-07T09%3A55%3A23Z&sp=r"
            //],
            //            blob: ["https://www.spitball.co/item/%D7%94%D7%9E%D7%A1%D7%9C%D7%95%D7%9C-%D7%94%D7%90%D7%A7%D7%93%D7%9E%D7%99-%D7%94%D7%9E%D7%9B%D7%9C%D7%9C%D7%94-%D7%9C%D7%9E%D7%A0%D7%94%D7%9C/72742/%D7%90%D7%A0%D7%92%D7%9C%D7%99%D7%AA-%D7%9E%D7%AA%D7%A7%D7%93%D7%9E%D7%99%D7%9D/175573/%D7%9E-%D7%A0%D7%95%D7%A9%D7%90-410011-%D7%90%D7%A0%D7%92%D7%9C%D7%99%D7%AA-%D7%9E%D7%AA%D7%A7%D7%93%D7%9E%D7%99%D7%9D-%D7%9E%D7%95%D7%A2%D7%93-1-2.pdf/"
            //],
            type: "lijionk",
            blob: ["https://zboxstorage.blob.core.windows.net/zboxcahce/eaa2ec96-4b61-49cf-97da-6e1aaf45e9c3V4_0_.pdf.jpg?sv=2016-05-31&sr=b&sig=tpGJH2NVDgIR8j5xoC%2BG2UUk7%2F75nyB5OwKW4S9%2FliI%3D&st=2017-11-07T09%3A16%3A16Z&se=2017-11-07T09%3A37%3A16Z&sp=r", "https://zboxstorage.blob.core.windows.net/zboxcahce/eaa2ec96-4b61-49cf-97da-6e1aaf45e9c3V4_1_.pdf.jpg?sv=2016-05-31&sr=b&sig=XWjY5PbgnL%2B5FF3vf3tqQ3BzH4tWj%2FJQHh9g8eSZmM4%3D&st=2017-11-07T09%3A16%3A16Z&se=2017-11-07T09%3A37%3A16Z&sp=r", "https://zboxstorage.blob.core.windows.net/zboxcahce/eaa2ec96-4b61-49cf-97da-6e1aaf45e9c3V4_2_.pdf.jpg?sv=2016-05-31&sr=b&sig=u2C39%2B1E25zM9uZ363TzZhdpX5UUSddMMnpy0FMF7UA%3D&st=2017-11-07T09%3A16%3A16Z&se=2017-11-07T09%3A37%3A16Z&sp=r"],
            author: "Jamie Schneider",
            name: "Item name",
            date: "2014-01-03T14:02:12Z",

        };
    },
    flashcard(id) {
        return flashcard.get({ id: id });
        //return {
        //    name: 'Math 1172 - Exam 2',
        //    author: 'Ola Jankowska',
        //    pins: [0,2,5,1],
        //    cards:
        //    [{ "front": { "text": "∫arctan(x)" }, "cover": { "text": "", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/46fc2205-4553-4c89-900f-c2e1c48232e8.jpg" } }, { "front": { "text": "∫arcsec(x)" }, "cover": { "text": "", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/35544a46-799c-45df-95bd-27b99b4eae71.png" } }, { "front": { "text": "∫arcsin(x)" }, "cover": { "text": "", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/9f5fbadf-2f0f-4b08-acf3-94e806b7853f.jpg" } }, { "front": { "text": "Taylor Series of 1/(1-x)" }, "cover": { "text": "∑(k=1,∞) x^k\n\n1+x+x^2+x^3\n for |x|<1" } }, { "front": { "text": "Taylor Series of e^x" }, "cover": { "text": "", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/00ff71eb-084f-43e6-99b1-492830a9e8b7.jpg" } }, { "front": { "text": "Taylor Series of sin(x)" }, "cover": { "text": "", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/f731e732-0b87-49e3-8b4f-ea28b80757cf.jpg" } }, { "front": { "text": "Taylor Series of cos(x)" }, "cover": { "text": "R.O.C. = ∞", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/61c453ad-1d4f-4e6f-9d36-fa6143062f12.png" } }, { "front": { "text": "Taylor Polynomial" }, "cover": { "text": "", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/a1dc40e2-cce4-46e9-842e-39675bd7e686.png" } }, { "front": { "text": "lim x->0 of ln(x)" }, "cover": { "text": "-∞" } }, { "front": { "text": "Growth Rate Sequence" }, "cover": { "text": "ln(n) ≺ n^r ≺a^n ≺ n! ≺ n^n" } }, { "front": { "text": "If the sequence of partial sums {sn} has a limit L, the infinite series..." }, "cover": { "text": "converges to L." } }, { "front": { "text": "Does a bounded monotonic sequence converge or diverge?" }, "cover": { "text": "Converge." } }, { "front": { "text": "Geometric Series" }, "cover": { "text": "", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/5f5cf4b1-0343-4163-90bb-dd5b588836bd.jpg" } }, { "front": { "text": "A geometric series wil converge if r..." }, "cover": { "text": "|r| < 1" } }, { "front": { "text": "State how to use the divergence test" }, "cover": { "text": "", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/78b84d72-dc12-477a-b7a2-16a1cccd2f7d.jpg" } }, { "front": { "text": "Convergence of a Power Series" }, "cover": { "text": "", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/84b18e52-a7a9-4b14-99da-b847d8770bb9.png" } }, { "front": { "text": "What is the ratio test?" }, "cover": { "text": "", "image": "https://zboxstorage.blob.core.windows.net/zboxquestion/flashcard/8cadb0a9-f844-43f4-ad4f-53c0ac6c9efe.jpg" } }, { "front": { "text": "If a limit evaluates to infinity then it...;" }, "cover": { "text": "diverges" } }, { "front": { "text": "lim (x->∞) (1+1/x)^x" }, "cover": { "text": "e^x" } }, { "front": { "text": "{an}" }, "cover": { "text": "sequence" } }, { "front": { "text": "{sn}" }, "cover": { "text": "sequence of partial sums (s1+s2+s3....)" } }, { "front": { "text": "Limit of a sequence" }, "cover": { "text": "If lim{an} =L, the sequence converges to L." } }, { "front": { "text": "∑ak" }, "cover": { "text": "series" } }, { "front": { "text": "If a sequence of partial sums has a limit = L, the series will..." }, "cover": { "text": "converge to L" } }, { "front": { "text": "Formula to find out if a geometric series will converge or diverge." }, "cover": { "text": "(ar^(nº))/1-r \nif nº≠ 0" } }, { "front": { "text": "If ∑{ak} converges" }, "cover": { "text": "lim {ak} = 0 (divergence test)" } }, { "front": { "text": "Harmonic Series" }, "cover": { "text": "1/k\n•diverges" } }, { "front": { "text": "lim {a(k+1)/a(k)} = 1" }, "cover": { "text": "r=1 , Inconclusive (ratio test)" } }, { "front": { "text": "lim {a(k+1)/a(k)} = 0" }, "cover": { "text": "converge" } }, { "front": { "text": "| lim {a(k+1)/a(k)} | < 1" }, "cover": { "text": "converge" } }, { "front": { "text": "lim {a(k+1)/a(k)} < 1" }, "cover": { "text": "converge" } }, { "front": { "text": "Where does a series always converge?" }, "cover": { "text": "At its center" } }]
        //};
    },
    quiz(id) {
        return { name: "quiz" };
    }
};

export default {
    activateFunction: {
        ask({ source, university, course, term, page, sort, q:userText }) {
             return  search.getQna({ source, university, course, term, page, sort, userText });
        },
        note({ source, university, course, term, page, sort }) {
               return search.getDocument({ source, university, course, term, page, sort });
        },
        flashcard({ source, university, course, term, page, sort }) {
                return search.getFlashcard({ source, university, course, term, page, sort });
        },
        tutor({ term, filter, sort, page,location }) {
            return search.getTutor({ term, filter, sort, location, page })
        },
        job({ term, filter, sort, jobType:facet,page,location }) {
               return search.getJob({ term, filter, sort, location, facet,page });
        },
        book({ term, page }) {
               return search.getBook({ term, page });
        },
        bookDetails({ type, isbn13 }) {
               return search.getBookDetails({ type, isbn13 })
        },
        food({ term, filter, page: nextPageToken,location }) {
           
                if (nextPageToken) {
                    return search.getFood({ nextPageToken })
                }else{
                   return search.getFood({ term, filter, location })
                }
        }
    },
    getPreview({ type, id }) {
        let previewFunc = previewMap[type] ? previewMap[type] : previewMap.item;

        return previewFunc(id);
    }
}