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
                        :rules="[rules.required, rules.minimumChars]"
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
                        :rules="[rules.required, rules.minimumChars]"
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
                :rules="[rules.required, rules.email]"
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
                :rules="[rules.required, rules.minimumCharsPass]"
                :label="labels['password']"
                :error-messages="errorMessages.password"
                placeholder=" "
                type="password"
                :hint="passHint"
            >
            </v-text-field>
        </div>
    </div>
</template>

<script>
import { mapGetters } from "vuex";

import { validationRules } from '../../../../../../services/utilities/formValidationRules';

export default {
    data() {
        return {
            gender: "male",
            password: "",
            firstName:'',
            lastName:'',
            email: '',
            firstNameError:'',
            lastNameError:'',
            labels: {
                fname: this.$t('loginRegister_setemailpass_first'),
                lname: this.$t('loginRegister_setemailpass_last'),
                email: this.$t('loginRegister_setemailpass_input_email'),
                genderMale: this.$t('loginRegister_setemailpass_male'),
                genderFemale: this.$t('loginRegister_setemailpass_female'),
                password: this.$t('loginRegister_setemailpass_input_pass'),
            },
            score: {
                default: 0,
                required: false
            },
            passScoreObj: {
                0: { name: this.$t("login_password_indication_weak"), className: "bad" },
                1: { name: this.$t("login_password_indication_weak"), className: "bad" },
                2: { name: this.$t("login_password_indication_strong"), className: "good" },
                3: { name: this.$t("login_password_indication_strong"), className: "good" },
                4: { name: this.$t("login_password_indication_strongest"), className: "best" }
            },
            rules: {
                required: (value) => validationRules.required(value),
                minimumChars: (value) => validationRules.minimumChars(value, 2),
                minimumCharsPass: (value) => validationRules.minimumChars(value, 8),
                email: value => validationRules.email(value)
            },
        };
    },
    computed: {
        ...mapGetters(["getErrorMessages", "getPassScoreObj"]),
        passHint() {
            if (this.password.length > 0) {
                this.changeScore()
                return `${this.passScoreObj[this.score].name}`;
            }
            return null
        },
        errorMessages() {
            return this.getErrorMessages;
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
};
</script>