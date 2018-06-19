import accountService from '../../services/accountService';
import {cashOutCards} from './consts'
import cashOutCard from './cashOutCard/cashOutCard.vue'
import {mapGetters} from 'vuex'

export default {
    components: {cashOutCard},
    props: {},
    data() {
        return {
            activeTab: 1,
            walletData: {},
            cashOut: true,
            search: '',
            pagination: {
                rowsPerPage: 6
            },
            selected: [],
            allTransactionsHeaders: [
                {text: 'Date', align: 'left', value: 'date', sortable: true, showOnMobile: true},
                {text: 'Action', align: 'left', value: 'action', sortable: true, showOnMobile: true},
                {text: 'Type', align: 'left', value: 'type', sortable: true, showOnMobile: false},
                {text: 'Amount', align: 'right', value: 'amount', sortable: true, showOnMobile: true},
                {text: 'Balance', align: 'right', value: 'balance', sortable: true, showOnMobile: false},
            ],
            allBalanceHeaders: [
                {text: '', align: 'left', value: 'name',showOnMobile: true},
                {text: 'Points', align: 'left', value: 'points',align: 'right',showOnMobile: true},
                {text: 'Value', align: 'left', value: 'value',align: 'right',showOnMobile: true},
            ],
            headers: {
                balances: [],
                transactions: []
            },
            items: [],
            cashOutOptions: cashOutCards
        }
    },
    methods: {
        changeActiveTab(tabId) {
            this.activeTab = tabId;
            if (tabId === 1) {
                this.items = this.getBalances();
            } else {
                this.items = this.getTransactions();
            }
        },
        getBalances() {
            return [
                {
                    type: 'Awarded',
                    points: '250',
                    value: '25.00',
                },
                {
                    type: 'Earned (Withdrawable)',
                    points: '987',
                    value: '98.70'
                },
                {
                    type: 'Pending',
                    points: '10',
                    value: '1.00',
                },
                {
                    type: 'Staked',
                    points: '-25',
                    value: '-2.50',
                },
                {
                    type: 'Total',
                    points: '1,244',
                    value: '-2.50',
                }];
        },
        getTransactions() {
            return [
                {
                    date: '12/2/18',
                    action: 'Sign up',
                    type: 'Awarded',
                    amount: '100',
                    balance: '100'
                },
                {
                    date: '24/3/18',
                    action: 'Answer',
                    type: 'Earned',
                    amount: '5',
                    balance: '105'
                },
                {
                    date: '24/3/18',
                    action: 'FB post',
                    type: 'Awarded',
                    amount: '25',
                    balance: '130'
                },
                {
                    date: '24/3/18',
                    action: 'Answer',
                    type: 'Earned',
                    amount: '5',
                    balance: '135'
                },
                {
                    date: '12/5/18',
                    action: 'Question',
                    type: 'Paid',
                    amount: '-15',
                    balance: '120'
                },
                {
                    date: '16/5/18',
                    action: 'Question',
                    type: 'Stake',
                    amount: '-5',
                    balance: '115'
                },
                {
                    date: '18/5/18',
                    action: 'Question',
                    type: 'Stake',
                    amount: '-5',
                    balance: '110'
                },
                {
                    id: 8,
                    date: '20/5/18',
                    action: 'Question',
                    type: 'Paid',
                    amount: '-15',
                    balance: '95'
                },
            ]
        }
    },
    computed: {
        ...mapGetters(["accountUser"]),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        pages() {
            return this.pagination.rowsPerPage ? Math.ceil(this.items.length / this.pagination.rowsPerPage) : 0
        },
    },
    created() {
        this.items = this.getBalances();
        this.headers.transactions = this.$vuetify.breakpoint.xsOnly ? this.allTransactionsHeaders.filter(header => header.showOnMobile === true) : this.allTransactionsHeaders;
        this.headers.balances = this.$vuetify.breakpoint.xsOnly ? this.allBalanceHeaders.filter(header => header.showOnMobile === true) : this.allBalanceHeaders;

    }
}