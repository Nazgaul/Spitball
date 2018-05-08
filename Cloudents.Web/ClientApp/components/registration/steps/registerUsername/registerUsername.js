import {mapGetters, mapActions} from 'vuex'
import stepTemplate from '../stepTemplate.vue'

export default {
    components:{stepTemplate},
    data() {
        return {
            countryCodesList: ['001', '002', '003'],
            codeSent: false,
            username: ''
        }
    },
    computed: {
        ...mapGetters(['getUserName']),
    },
    methods: {
        ...mapActions(['updateUserName']),
        next() {
            this.updateUserName(this.username);
            this.$emit('next');
        }
    },
    created: function () {
        this.username = this.getUserName
    }
}