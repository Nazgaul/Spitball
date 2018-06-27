import walletService from '../../services/walletService';
import {
    cashOutCards
} from './consts';
import cashOutCard from './cashOutCard/cashOutCard.vue'
import {
    mapGetters
} from 'vuex'

export default {
    components: {
        cashOutCard
    },
    props: {},
    data() {
        return {
            activeTab: 1,
            walletData: {},
            cashOut: false,
            search: '',
            cash: 0,
            pagination: {
                rowsPerPage: 6
            },
            selected: [],
            allTransactionsHeaders: [{
                    text: 'Date',
                    align: 'left',
                    value: 'date',
                    sortable: true,
                    showOnMobile: true
                },
                {
                    text: 'Action',
                    align: 'left',
                    value: 'action',
                    sortable: true,
                    showOnMobile: true
                },
                {
                    text: 'Type',
                    align: 'left',
                    value: 'type',
                    sortable: true,
                    showOnMobile: false
                },
                {
                    text: 'Amount',
                    align: 'right',
                    value: 'amount',
                    sortable: true,
                    showOnMobile: true
                },
                {
                    text: 'Balance',
                    align: 'right',
                    value: 'balance',
                    sortable: true,
                    showOnMobile: false
                },
            ],
            allBalanceHeaders: [{
                    text: '',
                    align: 'left',
                    value: 'name',
                    showOnMobile: true
                },
                {
                    text: 'Points',
                    align: 'left',
                    value: 'points',
                    align: 'right',
                    showOnMobile: true
                },
                {
                    text: 'Value',
                    align: 'left',
                    value: 'value',
                    align: 'right',
                    showOnMobile: true
                },
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
                this.getBalances();
            } else {
                this.getTransactions();
            }
        },
        getBalances() {
            walletService.getBalances()
                .then((response) => {
                        this.items = response.data;
                        var total = {
                            points: 0,
                            type: "total",
                            value: 0
                        };

                        var earnedVal;
                        for (var item of this.items) {
                            if (item.type !== 'pending') {
                                this.cash += item.value;

                                if (item.type === 'earned') {
                                    earnedVal = item.value
                                }
                            }
                            total.points = total.points + item.points;
                            total.value = total.value + item.value;
                        }
                        this.cash = Math.min(this.cash, earnedVal);
                        this.items.push(total);
                    },
                    error => {
                        console.error('error getting balance:', error)
                    }
                )
        },
        getTransactions() {
            walletService.getTransactions().then(response => {
                    this.items = response.data
                },
                error => {
                    console.error('error getting transactions:', error)
                })
        }
    },
    computed: {
        ...mapGetters(["accountUser"]),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        pages() {
            return this.pagination.rowsPerPage ? Math.ceil(this.items.length / this.pagination.rowsPerPage) : 0
        }
    },

    created() {
        this.getBalances();
        this.headers.transactions = this.$vuetify.breakpoint.xsOnly ? this.allTransactionsHeaders.filter(header => header.showOnMobile === true) : this.allTransactionsHeaders;
        this.headers.balances = this.$vuetify.breakpoint.xsOnly ? this.allBalanceHeaders.filter(header => header.showOnMobile === true) : this.allBalanceHeaders;

    }
}