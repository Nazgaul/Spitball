import { router } from "../../main";
import wyzantImg from '../home/img/wyzant.png'
import cheggImg from '../home/img/chegg.png'
import wayUpImg from '../home/img/wayup.png'
import courseHeroImg from '../home/img/course-hero.png'
import studyBlueImg from '../home/img/study-blue.png'
import tutorMeImg from './img/partners/TutorMe.png'
import zipRecruiterImg from './img/partners/ZipRecuiter.png'
import jobs2careersImg from './img/partners/Jobs2careers.png'
import careerjetImg from './img/partners/Careerjet.png'
import indeedImg from './img/partners/indeed.png'

const tutorGeneralData = {
    name: 'tutor',
    titleHtml: 'Get a Tutor <br class="hidden-sm-and-up"/><b>Now!</b>',
    bodyHtml: 'Qualified tutors at an affordable price, just for you.',
    submitButtonText: "Get help",
    placeholder: 'What do you need help with?',
    submissionCallback: (val) => {
        router.push({path: "/tutor", query: {q: val}});
    },
    partnersImages:[
        {name: 'tutorMe', source: tutorMeImg},
        {name: 'wyzant', source:wyzantImg},
        {name: 'chegg', source:cheggImg}],
    bottomImage: "tutorBottomImg.png"
};

const notesGeneralData = {
    name: 'note',
    titleHtml: 'Get the Notes <br class="hidden-sm-and-up"/>You <b>Need</b>',
    bodyHtml: 'Notes, study guides, exams and more from students at your school.',
    submitButtonText: "Search",
    placeholder: 'Where do you go to school?',
    submissionCallback: (val) => {
        router.push({path: "/note", query: {q: val}});
    },
    partnersImages:[
        {name: 'courseHero', source: courseHeroImg},
        {name: 'studyBlue', source: studyBlueImg},
        {name: 'chegg', source: cheggImg}],
    bottomImage:"notesBottomImg.png"
};

const jobsGeneralData = {
    titleHtml: 'You Don\'t Have to <br class="hidden-sm-and-up"/><b>Be Broke</b>',
    bodyHtml: 'Thousands of companies are hiring students right now.',
    submitButtonText: "Get paid",
    placeholder: 'What do you want to do?',
    submissionCallback: (val) => {
        router.push({path: "/job", query: {q: val}});
    },
    partnersImages:[
        {name: 'zipRecruiter', source: zipRecruiterImg},
        {name: 'jobs2careers', source: jobs2careersImg},
        {name: 'careerjet', source: careerjetImg},
        {name: 'wayUp', source: wayUpImg},
        {name: 'indeed', source: indeedImg}],
    bottomImage: "jobsBottomImg.png"
};

export const landingPagesData = {
    tutorV1: {
        type: 'tutor',
        wrappingClass: 'v1',
        background: 'tutorBgV1.jpg',
        ...tutorGeneralData
    },
    tutorV2: {
        type: 'tutor',
        wrappingClass: 'v2',
        background: '',
        ...tutorGeneralData
    },
    notesV1: {
        type: 'notes',
        wrappingClass: 'v1',
        background: 'studyDocs.jpg',
        ...notesGeneralData
    },
    notesV2: {
        type: 'notes',
        wrappingClass: 'v2',
        background: '',
        ...notesGeneralData
    },
    jobsV1: {
        type: 'job',
        wrappingClass: 'v1',
        background: 'jobs.jpg',
        ...jobsGeneralData
    },
    jobsV2: {
        type: 'job',
        wrappingClass: 'v2',
        background: '',
        ...jobsGeneralData
    }
};