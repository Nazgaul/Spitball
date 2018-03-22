import { router } from "../../main";
import wyzantImg from '../home/img/wyzant.png'
import cheggImg from '../home/img/chegg.png'
import TutorMeImg from './img/partners/TutorMe.png'

export const landingPagesData = {
    tutorV1: {
        name: 'tutor',
        wrappingClass: 'landing-tutor-v1',
        background: 'tutorBgV1.jpg',
        titleHtml: 'Get a Tutor <br class="hidden-sm-and-up"/><b>Now!</b>',
        bodyHtml: 'Qualified tutors at an affordable price, just for you.',
        submitButtonText: "Get help",
        placeholder: 'What do you need help with?',
        resultsPath: '/tutor',
        submissionCallback: (val) => {
            router.push({path: "/tutor", query: {q: val}});
        },
        partnersImages:[TutorMeImg, wyzantImg, cheggImg]
    },
    tutorV2: {
        name: 'tutor',
        wrappingClass: 'landing-tutor-v2',
        background: '',
        titleHtml: 'Get a Tutor <br class="hidden-sm-and-up"/><b>Now!</b>',
        bodyHtml: 'Qualified tutors at an affordable price, just for you.',
        submitButtonText: "Get help",
        placeholder: 'What do you need help with?',
        resultsPath: '/tutor',
        submissionCallback: (val) => {
            router.push({path: "/tutor", query: {q: val}});
        },
        partnersImages:[TutorMeImg, wyzantImg, cheggImg]
    },
    studyDocs: {
        name: 'note',
        wrappingClass: 'landing-study-docs',
        background: 'studyDocs.jpg',
        titleHtml: 'Get the <br/>Notes <br/><b>you need</b>',
        bodyHtml: 'We’ll bring you the best notes, study guides, <br class="hidden-xs-only"/>exams and more - all in one place, all from <br class="hidden-xs-only"/>YOUR school!',
        submitButtonText: "",
        placeholders: {uni: 'Where do you go to school?', term: 'Enter your subject'},
        submissionCallback: (val) => {
            router.push({path: "/note", query: {q: val}});
        },
        partnersImages:[]
    },
    jobs: {
        name: 'job',
        wrappingClass: 'landing-jobs',
        background: 'jobs.jpg',
        titleHtml: 'You Don\'t <br/>Have <br/><b>to Be Broke</b>',
        bodyHtml: 'Spitball brings you the best job opportunities <br class="hidden-xs-only"/> tailored specifically for students',
        submitButtonText: "",
        placeholders: {term: 'What do you want to do?'},
        submissionCallback: (val) => {
            router.push({path: "/job", query: {q: val}});
        },
        partnersImages:[]
    }
}