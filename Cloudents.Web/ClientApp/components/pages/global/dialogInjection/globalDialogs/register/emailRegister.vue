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
import authMixin from '../../../../../mixins/authMixin'

export default {
    mixins: [authMixin],
    data() {
        return {
            gender: "male",
            password: "",
            firstName:'',
            lastName:'',
            email: '',
        };
    },
    created() {
        this.$loadScript("https://unpkg.com/zxcvbn@4.4.2/dist/zxcvbn.js");
    }
};
</script>