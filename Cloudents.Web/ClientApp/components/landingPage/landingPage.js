import statistics from './helpers/statisticsData.vue';
import landingFooter from './helpers/landingFooter.vue'
import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
import questionService from "../../services/questionService";
import { mapGetters, mapActions } from 'vuex';
import debounce from "lodash/debounce";
export default {
    name: "landingPage",
    components: {
        statistics,
        landingFooter,
        sbDialog,
    },
    data() {
        return {
            isFocused: false,
            subjectList: [],
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
            if (this.search && this.search > 0) {
                return true
            }
        },
        binding () {
            const binding = {};
            if (this.$vuetify.breakpoint.xsOnly) {
                binding.column = true
            }
            return binding
        }

    },
    watch: {
        search: debounce(function () {
            if (!!this.search && this.search.length > 2) {
                this.selectedSubject = this.search;
                console.log('updated subject')
            }
            if (this.search === "") {
                this.clearData();
            }
        }, 250)
    },
    methods: {
        getAllSubjects() {
            questionService.getSubjects().then((response) => {
                this.subjectList = response.data.map(a => a.subject)
                console.log(this.subjectList)
            });
        },
        clearData(search, university) {
            search = '';
            university = undefined;
        },
        updateVideoId(videoID) {
            if (this.$vuetify.breakpoint.xs) {
                // this.playerWidth = '100%';
                // this.playerHeight = '100%';

            }else{
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
        updateSubject(val){
            this.selectedSubject = val;
            console.log('!!!subj', this.selectedSubject)
        },
        goToResulstQuestionsPage(val){
            this.$router.push({path: 'ask', query: val});
        },
        goToResultDocumentsPage(val){
            this.$router.push({name: 'note', query: val});
        }

    },
    created(){
        this.getAllSubjects();
    },
    filters: {
        boldText(value, search) {
            let match;
            //mark the text bold according to the search value
            if (!value) return '';
            if (!!search) {
                match = value.toLowerCase().indexOf(search.toLowerCase()) > -1;
            }
            if (match) {
                let startIndex = value.toLowerCase().indexOf(search.toLowerCase());
                let endIndex = search.length;
                let word = value.substr(startIndex, endIndex)
                return value.replace(word, '<b>' + word + '</b>')
            } else {
                return value;
            }

        }
    },
}