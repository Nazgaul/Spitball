import {mapActions, mapGetters} from 'vuex'
import nav from '../../../services/navigation/vertical-navigation/nav'
import particles from '../../particles/particles.vue';
export default {
    components: {
        particles
    },
    data() {
        return {
            bannerData: {
                title: "",
                text: "",
                lineColor: "#ffffff",
            },
        }
    },

    watch: {
        '$route': 'getBannerData',
    },
    computed: {
        ...mapGetters({
            campaignName: 'getCampaignName',
        }),
    },
    methods: {
        ...mapActions(['hideRegistrationBanner', 'updateCampaign' ]),
    getBannerData() {
            let route = this.$route;
            let path = route.path.slice(1);
            let query = route.query;
            this.bannerData = nav[path].banner(path, query);
            this.updateCampaign(query.promo);
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

