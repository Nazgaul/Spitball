export default {
    data() {
        return {
            passScoreObj: {
                0: { name: this.$t("login_password_indication_weak"), className: "bad" },
                1: { name: this.$t("login_password_indication_weak"), className: "bad" },
                2: { name: this.$t("login_password_indication_strong"), className: "good" },
                3: { name: this.$t("login_password_indication_strong"), className: "good" },
                4: { name: this.$t("login_password_indication_strongest"), className: "best" }
            },
            score: {
				default: 0,
				required: false
			},
        }
    },
    computed: {
        passHint() {
            if (this.password.length > 0) {
                this.changeScore()
                return `${this.passScoreObj[this.score].name}`;
            }
            return null
        },
        hintClass() {
            if (this.passHint) {
                return this.passScoreObj[this.score].className;
            }
            return null
        }
    },
    methods: {
        changeScore() {
            this.score = global.zxcvbn(this.password).score;
        }
    },
    created() {
        this.$loadScript("https://unpkg.com/zxcvbn@4.4.2/dist/zxcvbn.js");
    }
}