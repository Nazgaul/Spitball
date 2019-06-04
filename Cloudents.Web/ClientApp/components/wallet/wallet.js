import walletService from '../../services/walletService';
import { cashOutCards } from './consts';
import cashOutCard from './cashOutCard/cashOutCard.vue';
import { mapGetters } from 'vuex';
import { LanguageService } from "../../services/language/languageService";

export default {
    components: {
        cashOutCard
    },
    props: {},
    data() {
        return {
            activeTab: 1,
            isRtl: global.isRtl,
            active: null,
            cashOut: false,
            search: '',
            cash: 0,
            earnedPoints: 0,
            pagination: {
                rowsPerPage: 6
            },          
            selected: [],
            allTransactionsHeaders: [{
                text: LanguageService.getValueByKey('wallet_Date'),
                align: 'left',
                value: 'date',
                sortable: false,
                showOnMobile: true
            },
                {
                    text: LanguageService.getValueByKey('wallet_Action'),
                    align: 'left',
                    value: 'action',
                    sortable: false,
                    showOnMobile: true
                },
                {
                    text: LanguageService.getValueByKey('wallet_Type'),
                    align: 'left',
                    value: 'type',
                    sortable: false,
                    showOnMobile: false
                },
                {
                    text: LanguageService.getValueByKey('wallet_Amount'),
                    align: 'left',
                    value: 'amount',
                    sortable: false,
                    showOnMobile: true
                },
                {
                    text: LanguageService.getValueByKey('wallet_Balance'),
                    align: 'left',
                    value: 'balance',
                    sortable: false,
                    showOnMobile: false
                }
            ],
            allBalanceHeaders: [{
                text: '',
                align: 'left',
                value: 'name',
                showOnMobile: true
            },
                {
                    text: LanguageService.getValueByKey('wallet_Tokens'),
                    value: 'points',
                    align: 'left',
                    showOnMobile: true
                },
                {
                    text: LanguageService.getValueByKey('wallet_Value'),
                    value: 'value',
                    align: 'left',
                    showOnMobile: true
                }
            ],
            headers: {
                balances: [],
                transactions: []
            },
            items: [],
            cashOutOptions: cashOutCards,
            walletData: []
        };
    },
    methods: {
        changeActiveTab(tabId) {
            this.activeTab = tabId;
            if (tabId === 1) {
                this.getBalances();
            } else if(tabId === 2){
                this.getTransactions();
            }

        },
        gotToCashOutTab () {
            this.active = 'tab-3';
        },

        getBalances() {
            walletService.getBalances()
                .then((response) => {
                        let earnedVal;
                        const total = {
                            points: 0,
                            type: 'total',
                            value: 0,
                            name: LanguageService.getValueByKey("wallet_Total")
                        };
                        this.items = response.data;
                        this.items = this.items.map((item) => {
                            item.value = item.points.toFixed(2);
                            if (item.type.toLowerCase() !== 'pending') {
                                parseFloat(item.value);
                                this.cash += parseFloat(item.value);
                                if (item.type.toLowerCase() === 'earned') {
                                    earnedVal = parseFloat(item.value);
                                    this.earnedPoints = parseFloat(item.points);
                                }
                            }
                            total.points = total.points + parseFloat(item.points);
                            total.value = total.value + parseFloat(item.value);
                            return item;
                        });
                        this.cash = Math.min(this.cash, earnedVal);
                        total.value = total.value.toFixed(2);
                        this.items.push(total);
                        this.walletData = [...this.items];
                    },
                    error => {
                        console.error('error getting balance:', error);
                    }
                );
        },
        getTransactions() {
            walletService.getTransactions().then(response => {
                    this.items = response.data;
                },
                error => {
                    console.error('error getting transactions:', error);
                });
        },
        recalculate(){
            this.getBalances();
            this.cashOut = false;
        }
    },
    computed: {
        ...mapGetters(["accountUser"]),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        pages() {
            return this.pagination.rowsPerPage ? Math.ceil(this.items.length / this.pagination.rowsPerPage) : 0;
        },
        calculatedEarnedPoints(){
            let typesDictionary = {};
            let earned = 0;
            this.walletData.forEach((item) => {
                typesDictionary[item.type] = item.points;
            });
            let reduce = typesDictionary["Stake"] + typesDictionary["Spent"];
            if(reduce < 0){
                earned = typesDictionary["Earned"] + reduce;
            }else{
                earned = typesDictionary["Earned"];
            }
            return earned;
        },
        
    },
    created() {
        this.getBalances();
        this.headers.transactions = this.$vuetify.breakpoint.xsOnly ? this.allTransactionsHeaders.filter(header => header.showOnMobile === true) : this.allTransactionsHeaders;
        this.headers.balances = this.$vuetify.breakpoint.xsOnly ? this.allBalanceHeaders.filter(header => header.showOnMobile === true) : this.allBalanceHeaders;
    }
}