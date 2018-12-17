import statistics from './helpers/statisticsData.vue';
import landingFooter from './helpers/landingFooter.vue'
import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
import questionService from "../../services/questionService";
import { mapGetters, mapActions } from 'vuex';
import debounce from "lodash/debounce";
import { LanguageService } from "../../services/language/languageService";


export default {
    name: "landingPage",
    components: {
        statistics,
        landingFooter,
        sbDialog,

    },
    data() {
        return {
            schoolNamePlaceholder: LanguageService.getValueByKey('uniSelect_type_school_name_placeholder'),
            isFocused: false,
            selectedSubject: '',
            search: '',
            items: [
                {
                    src: 'https://cdn.vuetifyjs.com/images/carousel/squirrel.jpg'
                },
                {
                    src: 'https://cdn.vuetifyjs.com/images/carousel/sky.jpg'
                },
                {
                    src: 'https://cdn.vuetifyjs.com/images/carousel/bird.jpg'
                },
                {
                    src: 'https://cdn.vuetifyjs.com/images/carousel/planet.jpg'
                }
            ],
            youTubeVideoId: '',
            playerVisible: false,
            playerWidth: '',
            playerHeight: '',
            universityModel: '',
            searchUni: '',
        }
    },
    props: {
        propName: {
            type: Number,
            default: 0
        },
    },
    computed: {
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
            get(){
               let list = this.getSubjectsList();
               return list
            },
            set(){

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
        updateSubject(val) {
            this.selectedSubject = val;
        },
        goToResulstQuestionsPage(val) {
            this.$router.push({path: 'ask', query: {"source": val.id}});
        },
        goToResultDocumentsPage(val) {
            this.$router.push({name: 'note', query: val});
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
            if(value.subject){
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