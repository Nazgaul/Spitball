import accountService from '../../services/accountService';
import {mapGetters} from 'vuex'

export default {
    props: {},
    data() {
        return {
            activeTab: 1,
            walletData: {},

            search: '',
            pagination: {
                rowsPerPage: 6
            },
            selected: [],
            headers: [
                {text: '', align: 'left', value: 'id', sortable: false},
                {text: 'Date', align: 'left', value: 'date', sortable: false},
                {text: 'Action', align: 'left', value: 'action', sortable: false},
                {text: 'Type', align: 'left', value: 'type', sortable: false},
                {text: 'Amount', align: 'right', value: 'amount', sortable: false},
                {text: 'Balance', align: 'right', value: 'balance', sortable: false},
            ],
            items: [
                {
                    id: 1,
                    date: '12/2/18',
                    action: 'Sign up',
                    type: 'Awarded',
                    amount: '100 pt',
                    balance: '100 pt'
                },
                {
                    id: 2,
                    date: '24/3/18',
                    action: 'Answer',
                    type: 'Earned',
                    amount: '5 pt',
                    balance: '105 pt'
                },
                {
                    id: 3,
                    date: '24/3/18',
                    action: 'FB post',
                    type: 'Awarded',
                    amount: '25 pt',
                    balance: '130 pt'
                },
                {
                    id: 4,
                    date: '24/3/18',
                    action: 'Answer',
                    type: 'Earned',
                    amount: '5 pt',
                    balance: '135 pt'
                },
                {
                    id: 5,
                    date: '12/5/18',
                    action: 'Question',
                    type: 'Paid',
                    amount: '-15 pt',
                    balance: '120 pt'
                },
                {
                    id: 6,
                    date: '16/5/18',
                    action: 'Question',
                    type: 'Stake',
                    amount: '-5 pt',
                    balance: '115 pt'
                },
            ]
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
        pages() {
            return this.pagination.rowsPerPage ? Math.ceil(this.items.totalItems / this.pagination.rowsPerPage) : 0
        }
    },
    created() {
        var self = this;
    }
}