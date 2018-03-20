import { router } from "../../main";
export const landingPagesData = {
    tutorV1: {
        name: 'tutor',
        wrappingClass: 'landing-tutor-v1',
        background: 'tutorBgV1.jpg',
        titleHtml: 'Get <br/>a tutor <br/>now!',
        bodyHtml: 'We’ve teamed up with the most trusted  <br class="hidden-xs-only"/>tutoring services so you can find the most  <br class="hidden-xs-only"/>affordable and suitable tutor for YOU.',
        placeholders: {term: 'What do you need help with?'},
        resultsPath: '/tutor',
        submissionCallback: (val) => {
            router.push({path: "/tutor", query: {q: val}});
        }
    },
    tutorV2: {
        name: 'tutor',
        wrappingClass: 'landing-tutor-v2',
        background: 'tutorBgV2.jpg',
        titleHtml: 'Get <br/>the tutor <br/>you Need',
        bodyHtml: 'We teamed up with the most trusted <br class="hidden-xs-only"/>tutoring services out there so you can <br class="hidden-xs-only"/>find the perfect tutor for YOU.',
        placeholders: {term: 'What do you need help with?'},
        submissionCallback: (val) => {
            router.push({path: "/tutor", query: {q: val}});
        }
    },
    studyDocs: {
        name: 'note',
        wrappingClass: 'landing-study-docs',
        background: 'studyDocs.jpg',
        titleHtml: 'Get the <br/>Notes <br/><b>you need</b>',
        bodyHtml: 'We’ll bring you the best notes, study guides, <br class="hidden-xs-only"/>exams and more - all in one place, all from <br class="hidden-xs-only"/>YOUR school!',
        placeholders: {uni: 'Where do you go to school?', term: 'Enter your subject'},
        submissionCallback: (val) => {
            router.push({path: "/note", query: {q: val}});
        }
    },
    jobs: {
        name: 'job',
        wrappingClass: 'landing-jobs',
        background: 'jobs.jpg',
        titleHtml: 'You Don\'t <br/>Have <br/><b>to Be Broke</b>',
        bodyHtml: 'Spitball brings you the best job opportunities <br class="hidden-xs-only"/> tailored specifically for students',
        placeholders: {term: 'What do you want to do?'},
        submissionCallback: (val) => {
            router.push({path: "/job", query: {q: val}});
        }
    }
}