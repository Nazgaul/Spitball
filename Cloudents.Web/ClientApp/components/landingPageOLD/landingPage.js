import statistics from './helpers/statisticsData.vue';
import landingFooter from './helpers/landingFooter.vue';
import landingHeader from './helpers/landingHeader.vue';

import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
import sbInput from "../question/helpers/sbInput/sbInput.vue"
import { mapGetters, mapActions, mapMutations } from 'vuex';
import debounce from "lodash/debounce";
import { LanguageService } from "../../services/language/languageService";
import { reviews, mobileReviews } from "./helpers/testimonials/testimonialsData"

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
            schoolNamePlaceholder: LanguageService.getValueByKey('landingPage_placeholder_uni'),
            subjectsPlaceholder: LanguageService.getValueByKey('landingPage_placeholder_subjects'),
            isFocused: false,
            selectedSubject: '',
            search: '',
            reviewItems: reviews,
            mobileReviewItems: mobileReviews,
            youTubeVideoId: '',
            SpitballVideoId: 'nreiplVSrWk',
            playerVisible: false,
            playerWidth: '',
            playerHeight: '',
            universityModel: '',
            searchUni: '',
            mobileSubjectsDialog: false,
            mobileUniDialog: false,
            isRtl: global.isRtl,
            dictionaryTypesEnum: this.getDictionaryPrefixEnum(),
            player: null,
            openDropdownUniMobile: false,
            openDropdownSubjectMobile: false

        };
    },
    props: {
        propName: {
            type: Number,
            default: 0
        }
    },
    computed: {
        ...mapGetters(['getDictionaryPrefix']),
        dictionaryType(){
            return this.getDictionaryPrefix;
        },

        statsData(){
            return this.statistics();
        },
        showBox() {
            if (this.search && this.search.length > 0) {
                return true;
            }
        },
        showBoxUni() {
            if (this.searchUni && this.searchUni.length > 0) {
                return true;
            }
        },
        isMobileView(){
            return this.$vuetify.breakpoint.width < 1024;
        },
        university: {
            get: function () {
                let schoolNameFromStore = this.getSchoolName();
                return schoolNameFromStore || this.universityModel;
            },
            set: function (newValue) {
                this.universityModel = newValue;
            }
        },
        universities() {

            return this.getUniversities();
        },
        subjectList: {
            get() {
                let list = this.getSubjectsList();
                return list;
            },
            set() {

            }
        },
        binding() {
            const binding = {};
            if (this.$vuetify.breakpoint.xsOnly) {
                binding.column = true;
            }
            return binding;
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
        }, 250),
        '$route'(){
            if(!!this.$route.query && this.$route.query.hasOwnProperty('type')){
                let dictionaryType = this.$route.query.type;
                //check if valid query type
                if(!!this.dictionaryTypesEnum[dictionaryType]){
                    this.changeDictionaryType(this.dictionaryTypesEnum[dictionaryType]);
                }
            }else{
                this.changeDictionaryType(this.dictionaryTypesEnum['learn']);
            }
        }
    },
    methods: {
        ...mapActions([
            "updateUniversities",
            "clearUniversityList",
            "updateSchoolName",
            "updateSubject",
            "getAllSubjects",
            "switchLandingPageText",
            "getStatistics"
        ]),
        ...mapGetters([
            "getUniversities",
            "getSchoolName",
            "getSubjectsList",
            "getDictionaryPrefixEnum",
            "statistics",
            "accountUser"
        ]),
        ...mapMutations(['UPDATE_SEARCH_LOADING']),
        readyPlayer (event) {
            this.player = event.target;
        },
        stopPlayer () {
            this.player.pauseVideo();
        },
        changeDictionaryType(val){
            this.scrollTop();
            this.switchLandingPageText(val);
        },
        changeUrlType(val){
            if(val === 'earn'){
                let typeObj = {
                    type: val
                };
                this.$router.push({query: typeObj});
            }else{
                this.$router.push({query: ``});
            }
            
        },
        scrollTop(){
            setTimeout(()=>{
                this.$nextTick(() => {
                    global.scrollTo(0, 0);
                });
            }, 200);
        },
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
                this.playerWidth = '100%';
                this.playerHeight = '450';

            } else {
                this.playerWidth = '640';
                this.playerHeight = '360';
            }
            this.youTubeVideoId = videoID;
            this.showVideoPlayer();
        },
        showVideoPlayer() {
            this.playerVisible = true;
        },
        hideVideoPlayer() {
            this.playerVisible = false;
            this.stopPlayer();
        },
        goToResulstQuestionsPage(val) {
            this.closeSubjectInputDialog();
            this.UPDATE_SEARCH_LOADING(true);
            setTimeout(()=>{
                this.$router.push({path: '/ask', query: {Source: val.id}});
            }, 200);
           
        },
        goToResultDocumentsPage(val) {
            this.closeUniInputDialog();
            setTimeout(()=>{
                this.$router.push({path: '/note', query: {term: val.text}});
            }, 200);
            
        },
        showMobileSubjectInput() {
            this.mobileSubjectsDialog = true;
            this.$nextTick(() => {
                setTimeout(() => {
                    this.openDropdownSubjectMobile = true;
                    }, 500);
            });
        },
        showMobileUniInput() {
            this.mobileUniDialog = true;
            this.$nextTick(() => {
                setTimeout(() => {
                    this.openDropdownUniMobile = true;
                }, 500);
            });
        },
        closeSubjectInputDialog() {
            this.mobileSubjectsDialog = false;
        },
        closeUniInputDialog() {
            this.mobileUniDialog = false;
        }
    },
    created() {
        let user = this.accountUser();
        if(!!this.$route.query && this.$route.query.hasOwnProperty('type')){
            let dictionaryuType = this.$route.query.type;
            //check if valid query type
            if(!!this.dictionaryTypesEnum[dictionaryuType]){
                this.changeDictionaryType(this.dictionaryTypesEnum[dictionaryuType]);
            }
        }
        if(!user){
            this.getAllSubjects();
            this.getStatistics();
        }else{
            // const isIl = global.country.toLowerCase() === 'il';
            // const defaultSubmitRoute = isIl ? {path: '/note'} : {path: '/ask'};
            const defaultSubmitRoute = {path: '/ask'};
            defaultSubmitRoute.query = this.$route.query;
            this.$router.push(defaultSubmitRoute);
        }

        
    },
    filters: {
        boldText(value, search) {
            //happens if string uni
            let valToFil = value;
            //happens if subject list
            if (value.subject) {
                valToFil = value.subject;
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
                let word = valToFil.substr(startIndex, endIndex);
                return valToFil.replace(word, '<b>' + word + '</b>');
            } else {
                return valToFil;
            }

        }
    },
}