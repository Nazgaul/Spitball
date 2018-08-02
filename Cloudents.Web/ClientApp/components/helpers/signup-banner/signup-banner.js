import {mapActions} from 'vuex'
import nav from '../../../services/navigation/vertical-navigation/nav'
import particles from '../../particles/particles.vue';
// import QuestionCard from "../../question/helpers/question-card/question-card";
export default {
    components: {
        particles
    },
    data() {
        return {
            bannerData: {
                title: "",
                text: "",
                lineColor: "#ffffff"
            },
        }
    },

    watch: {
        '$route': 'getBannerData',

    },
    methods: {
        ...mapActions(['hideRegistrationBanner']),


    getBannerData() {
            return this.bannerData = nav[this.$route.path.slice(1)].banner;
        }
    },
    created() {
        this.getBannerData()
    },
    filters: {
        bolder: function (value, query) {
            return value.replace(query, '<span class="bolder">' + query + '</span>')
        }
    }
}

