import questionCard from "../question/helpers/question-card/question-card.vue";
import userBlock from '../helpers/user-block/user-block.vue';
import {dollarCalculate} from "../../store/constants";
import accountService from '../../services/accountService';
import {mapGetters} from 'vuex'

export default {
    components: {questionCard, userBlock},
    props: {
        id: {Number}
    },
    data() {
        return {
            activeTab: 1,
            profileData: null,
            //TODO: what is that
            emptyState: {
                questions: {
                    text: 'You have a homework problem?',
                    btnText: 'Put it on sale…'
                },
                answers: {
                    text: 'You have a homework problem?',
                    btnText: 'Put it on sale…'
                }
            }
            
        }
    },
    methods: {
        changeActiveTab(tabId) {
            this.activeTab = tabId;
        },
        fetchData() {
            accountService.getProfile(this.id).then(({ data }) => {
                this.profileData = data;
            }, error => {
                window.location = "/error/notfound";
            })
        }
    },
    computed: {
        ...mapGetters(["accountUser"]),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        myAnswers() {
            return this.profileData.answers ? this.profileData.answers.map(i => {
                return {
                    ...i,
                    //user: this.profileData.user,
                    answersNum: i.answers,
                    filesNum: i.files,
                }
            }) : []
        },
        questions() {
            return this.profileData.questions ? this.profileData.questions.map(item => {
                return {
                    ...item,
                    user: this.profileData.user,
                    answersNum: item.answers,
                    filesNum: item.files,
                }
            }) : []
        },
        isMyProfile() {
            return this.accountUser && this.accountUser.id && this.profileData ? this.profileData.user.id == this.accountUser.id : false;
        },
        emptyStateData() {
            var questions = {
                text: 'Have a question on your homework?',
                boldText: 'Post it for SBL…',
                btnText: 'Ask Your Question',
                btnUrl: 'newQuestion'
            };
            var answers = {
                text: 'Help other students<br/> <b>and make money</b><br/> by answering questions.',
                btnText: 'Answer',
                btnUrl: 'home'
            };
            return this.activeTab === 1 ? questions : answers;

        }
    },
    watch: {
        '$route': 'fetchData'
    },
    created() {
        this.fetchData();
        
    }
}