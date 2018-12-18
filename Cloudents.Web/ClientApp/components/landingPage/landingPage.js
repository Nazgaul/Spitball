import statistics from './helpers/statisticsData.vue';
import landingFooter from './helpers/landingFooter.vue';
import landingHeader from './helpers/landingHeader.vue';

import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
import sbInput from "../question/helpers/sbInput/sbInput.vue"
import { mapGetters, mapActions } from 'vuex';
import debounce from "lodash/debounce";
import { LanguageService } from "../../services/language/languageService";
import { reviews } from "./helpers/testimonials/testimonialsData"

export default {
    name: "landingPage",
    components: {
        statistics,
        landingFooter,
        sbDialog,
        sbInput,
        landingHeader

    },
    data() {
        return {
            schoolNamePlaceholder: LanguageService.getValueByKey('uniSelect_type_school_name_placeholder'),
            isFocused: false,
            selectedSubject: '',
            search: '',
            reviewItems: reviews,
            youTubeVideoId: '',
            playerVisible: false,
            playerWidth: '',
            playerHeight: '',
            universityModel: '',
            searchUni: '',
            mobileSubjectsDialog: false,
            mobileUniDialog: false,
            isRtl: global.isRtl
        }
    },
    props: {
        propName: {
            type: Number,
            default: 0
        },
    },
    computed: {
        formattedReviews(){
           return  this.$vuetify.breakpoint.xsOnly ? [].concat(...this.reviewItems) :  this.reviewItems;

        },
        showBox() {
            if (this.search && this.search.length > 0) {
                return true
            }
        },
        showBoxUni() {
            if (this.searchUni && this.searchUni.length > 0) {
                return true
            }
        },
        university: {
            get: function () {
                let schoolNameFromStore = this.getSchoolName();
                return schoolNameFromStore || this.universityModel
            },
            set: function (newValue) {
                this.universityModel = newValue
            }
        },
        universities() {

            return this.getUniversities();
        },
        subjectList: {
            get() {
                let list = this.getSubjectsList();
                return list
            },
            set() {

            }
        },
        binding() {
            const binding = {};
            if (this.$vuetify.breakpoint.xsOnly) {
                binding.column = true
            }
            return binding
        },

    },
    watch: {
        search: debounce(function () {
            if (!!this.search && this.search.length > 2) {
                this.selectedSubject = this.search;
            }
            if (this.search === "") {
                this.clearData();
            }
        }, 250),

        searchUni: debounce(function () {
            if (!!this.searchUni && this.searchUni.length > 2) {
                this.updateUniversities(this.searchUni);
            }
            if (this.searchUni === "") {
                this.clearData();
            }
        }, 250)
    },
    methods: {
        ...mapActions([
            "updateUniversities",
            "clearUniversityList",
            "updateSchoolName",
            "updateSubject",
            "getAllSubjects"
        ]),
        ...mapGetters([
            "getUniversities",
            "getSchoolName",
            "getSubjectsList"
        ]),

        getAllUniversities() {
            //leave space
            this.updateUniversities(' ');
        },
        clearData(search, university) {
            search = '';
            university = undefined;
        },
        updateVideoId(videoID) {
            if (this.$vuetify.breakpoint.xs) {
                // this.playerWidth = '100%';
                // this.playerHeight = '100%';

            } else {
                // this.playerWidth = '1280';
                // this.playerHeight = '450';
            }
            this.youTubeVideoId = videoID;
            this.showVideoPlayer()
        },
        showVideoPlayer() {
            this.playerVisible = true;
        },
        hideVideoPlayer() {
            this.playerVisible = false;
        },
        goToResulstQuestionsPage(val) {
            this.$router.push({path: '/ask', query: {Source: val.id}});
        },
        goToResultDocumentsPage(val) {
            this.$router.push({path: '/note', query: {term: val.text}});
        },
        showMobileSubjectInput() {
            this.mobileSubjectsDialog = true
        },
        showMobileUniInput() {
            this.mobileUniDialog = true
        },
        closeSubjectInputDialog() {
            this.mobileSubjectsDialog = false
        },
        closeUniInputDialog() {
            this.mobileUniDialog = false
        }
    },
    created() {
        this.getAllSubjects()
    },
    filters: {
        boldText(value, search) {
            //happens if string uni
            let valToFil = value;
            //happens if subject list
            if (value.subject) {
                valToFil = value.subject
            }
            let match;
            //mark the text bold according to the search value
            if (!valToFil) return '';
            if (!!search) {
                match = valToFil.toLowerCase().indexOf(search.toLowerCase()) > -1;
            }
            if (match) {
                let startIndex = valToFil.toLowerCase().indexOf(search.toLowerCase());
                let endIndex = search.length;
                let word = valToFil.substr(startIndex, endIndex)
                return valToFil.replace(word, '<b>' + word + '</b>')
            } else {
                return valToFil;
            }

        }
    },
}