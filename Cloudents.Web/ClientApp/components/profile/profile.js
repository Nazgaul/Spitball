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
        }
    },
    computed: {
        ...mapGetters(["accountUser"]),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        myAnswers() {
            return this.profileData.answer ? this.profileData.answer.map(i => {
                return {
                    ...i,
                    user: this.profileData.user,
                    answersNum: i.answers,
                    filesNum: i.files,
                }
            }) : []
        },
        questions() {
            return this.profileData.ask ? this.profileData.ask.map(i => {
                return {
                    ...i,
                    user: this.profileData.user,
                    answersNum: i.answers,
                    filesNum: i.files,
                }
            }) : []
        },
        isMyProfile() {
            return this.accountUser && this.accountUser.id && this.profileData ? this.profileData.user.id == this.accountUser.id : false;
        },
        emptyStateData() {
            var questions = {
                text: 'You have<br/>a homework problem?',
                boldText: 'Put it on sale…',
                btnText: 'Ask Your Question',
                btnUrl: 'newQuestion'
            };
            var answers = {
                text: 'Help your friends,<br/>answer on their questions',
                boldText: 'and make some money',
                btnText: 'Answer',
                btnUrl: 'home'
            };
            return this.activeTab === 1 ? questions : answers;

        }
    },
    created() {
        var self = this;
        accountService.getProfile(this.id).then(function (response) {
            self.profileData = response.data;
        })
    }
}