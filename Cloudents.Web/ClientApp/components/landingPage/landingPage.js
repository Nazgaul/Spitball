import questionCard from '../question/helpers/question-card/question-card.vue';
import extendedTextArea from '../question/helpers/extended-text-area/extendedTextArea.vue';
import statistics from './helpers/statisticsData.vue';
import landingFooter from './helpers/landingFooter.vue'

import questionService from "../../services/questionService";
import { mapGetters, mapActions } from 'vuex';
import debounce from "lodash/debounce";
export default {
    name: "landingPage",
    components:{
        questionCard,
        extendedTextArea,
        statistics,
        landingFooter
    },
    data() {
        return {
            randomQuestionData:{
                color: 'green',
                user:{

                },
                subject:'test subject',
                price: '90'
            },
            value: {type: String},
            error: {},
            actionType:  'answer',
            isFocused: false,
            uploadUrl: {type: String},
            textAreaValue: '',
            errorTextArea:{},
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
            ]
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

    },
    watch:{
        search: debounce(function(){
            if(!!this.search && this.search.length > 2 ){
                this.selectedSubject = this.search;
                console.log('updated subject')
            }
            if(this.search === ""){
                this.clearData();
            }
        }, 250)
    },
    methods: {
        getAllSubjects(){
            questionService.getSubjects().then((response) => {
                this.subjectList = response.data.map(a => a.subject)
                console.log(this.subjectList)
            });
        },
        clearData(search, university){
            search = '';
            university = undefined;
        },

    },
    filters:{
        boldText(value, search){
            let match;
            //mark the text bold according to the search value
            if (!value) return '';
            if(!!search) {
                match = value.toLowerCase().indexOf(search.toLowerCase()) > -1;
            }
            if(match){
                let startIndex = value.toLowerCase().indexOf(search.toLowerCase());
                let endIndex = search.length;
                let word = value.substr(startIndex, endIndex)
                return value.replace(word, '<b>' + word + '</b>')
            }else{
                return value;
            }

        }
    },
}