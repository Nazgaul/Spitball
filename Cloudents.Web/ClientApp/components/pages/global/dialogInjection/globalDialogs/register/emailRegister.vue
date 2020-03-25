<template>
    <div>
        <div>
            <v-layout wrap class="widther">
                <v-flex xs12 sm6 class="mb-3 pr-sm-2">
                    <v-text-field
                        v-model="firstName"
                        class="input-fields"
                        color="#304FFE"
                        outlined
                        height="44"
                        dense
                        :label="labels['fname']"
                        :rules="[val => val.length > 2 || $t('formErrors_min_chars')]"
                        :error-messages="firstNameError"
                        placeholder=" "
                        autocomplete="nope"
                        type="text"
                    >
                    </v-text-field>
                </v-flex>

                <v-flex xs12 sm6 class="pl-sm-2 mb-3">
                    <v-text-field
                        v-model="lastName"
                        class="input-fields"
                        color="#304FFE"
                        outlined
                        height="44"
                        dense
                        :label="labels['lname']"
                        :rules="[val => val.length > 2 || $t('formErrors_min_chars')]"
                        :error-messages="lastNameError"
                        placeholder=" "
                        autocomplete="nope"
                        type="text"
                    >
                    </v-text-field>
                </v-flex>
            </v-layout>

            <v-text-field 
                v-model="email"
                class="widther input-fields" 
                color="#304FFE"
                outlined
                height="44" 
                dense
                :label="labels['email']"
                :error-messages="errorMessages.email"
                placeholder=" "
                type="email"
            >
            </v-text-field>

            <v-radio-group v-model="gender" row class="radioActive mt-n1" dense :mandatory="true">
                <v-radio :label="labels['genderMale']" value="male" on-icon="sbf-radioOn" off-icon="sbf-radioOff"></v-radio>
                <v-radio :label="labels['genderFemale']" value="female" on-icon="sbf-radioOn" off-icon="sbf-radioOff"></v-radio>
            </v-radio-group>

            <v-text-field 
                v-model="password"
                :class="[hintClass,'widther','input-fields','mb-3']"
                color="#304FFE"
                outlined
                height="44"
                dense
                :label="labels['password']"
                :error-messages="errorMessages.password"
                placeholder=" "
                type="password"
                :hint="passHint"
            >
            </v-text-field>

            <!-- <v-btn
                :loading="getGlobalLoading"
                large
                rounded
                block
                color="primary"
                class="white--text btn-login">
                    <span>{{$t('loginRegister_setphone_btn')}}</span>
            </v-btn> -->


        </div>

    </div>
</template>

<script>
import { mapActions, mapGetters, mapMutations } from "vuex";


export default {
    // components: { VueRecaptcha },
    data() {
        return {
            gender: "male",
            password: "",
            score: {
                default: 0,
                required: false
            },

            firstName:'',
            lastName:'',
            firstNameError:'',
            lastNameError:'',
            labels: {
                fname: this.$t('loginRegister_setemailpass_first'),
                lname: this.$t('loginRegister_setemailpass_last'),
                email: this.$t('loginRegister_setemailpass_input_email'),
                genderMale: this.$t('loginRegister_setemailpass_male'),
                genderFemale: this.$t('loginRegister_setemailpass_female'),
                password: this.$t('loginRegister_setemailpass_input_pass'),
            }
        };
    },
    computed: {
        ...mapGetters(["getEmail1","getGlobalLoading","getErrorMessages","getPassScoreObj"]),
        passHint() {
            if (this.password.length > 0) {
                let passScoreObj = this.getPassScoreObj;
                this.changeScore()
                return `${passScoreObj[this.score].name}`;
            }
            return null
        },
        errorMessages() {
            return this.getErrorMessages;
        },
        email: {
            get() {
                return this.getEmail1;
            },
            set(val) {
                this.updateEmail(val);
                this.setErrorMessages({})
            }
        },
        hintClass() {
            let passScoreObj = this.getPassScoreObj;
            if (this.passHint) {
                return passScoreObj[this.score].className;
            }
            return null
        }
    },
    methods: {
        ...mapActions(["updateEmail","emailSigning",'updateName', 'updateGender']),
        ...mapMutations(['setErrorMessages']),
        // onVerify(response) {
        //     this.recaptcha = response
        //     this.register()
        // },
        // onExpired() {
        //     this.recaptcha = ''
        //     this.$refs.recaptcha.reset();
        // },
        // register() {
        //     let paramObj = {
        //         password: this.password,
        //         recaptcha: this.recaptcha
        //     }
            // this.emailSigning(paramObj).then(() => {},() => {
            //     this.$refs.recaptcha.reset()
            // });
        // },
        // submit() {
        //     if(this.firstName.length > 1 && this.lastName.length > 1){
        //         this.$refs.recaptcha.execute()
        //     } else{
        //         if(this.firstName.length < 2 ){
        //             this.firstNameError = `${this.$t("formErrors_min_chars")} 2`
        //         }
        //         if(this.lastName.length < 2 ){
        //             this.lastNameError = `${this.$t("formErrors_min_chars")} 2`
        //         }
        //     }
        // },
        changeScore() {
            this.score = global.zxcvbn(this.password).score;
        }
    },
    created() {
        this.$loadScript("https://unpkg.com/zxcvbn@4.4.2/dist/zxcvbn.js");

    }
};
</script>