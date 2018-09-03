import { mapGetters, mapActions } from 'vuex';
import userBlock from '../helpers/user-block/user-block'
import askQuestionBtn from "../results/helpers/askQuestionBtn/askQuestionBtn.vue";

export default {
    name: "notificationCenter",
    components: {userBlock, askQuestionBtn},
    data() {
        return {
            items: [
                {
                    action: '15 min',
                    headline: 'Brunch this weekend?',
                    title: 'Your question was answered',
                    subtitle: "I'll be in your neighborhood doing errands this weekend. Do you want to hang out?",
                    type: 'answer'
                },
                {
                    action: '2 hr',
                    headline: 'Summer BBQ',
                    title: 'Your question was answered',
                    subtitle: "Wish I could come, but I'm out of town this weekend.",
                    type: 'answer'
                },
                {
                    action: '6 hr',
                    headline: 'Oui oui',
                    title: '',
                    subtitle: 'Do you have Paris recommendations? Have you ever been?',
                    type: 'answer'
                },
                {
                    action: '12 hr',
                    headline: 'Birthday gift',
                    title: 'Trevor Hansen',
                    subtitle: 'Have any ideas about what we should get Heidi for her birthday?',
                    type: 'rewarded'
                },
                {
                    action: '18hr',
                    headline: 'Recipe to try',
                    title: 'Britta Holt',
                    subtitle: 'We should eat this: Grate, Squash, Corn, and tomatillo Tacos.',
                    type: 'rewarded'
                }
            ]
        }
    },
    props: {
        isAsk: {
            type: Boolean,
            default: true
        },

    },
    computed: {
        ...mapGetters(['getNotifications',]),

    },
    methods: {},
    created() {
    }
}