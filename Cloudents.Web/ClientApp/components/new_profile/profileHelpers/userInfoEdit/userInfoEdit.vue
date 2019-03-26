<template>
    <v-card class="user-edit-wrap">
    <v-layout row>
        <v-flex xs3>
            <v-text-field
                    class="border-1"
                    :label="firstNameLabel"
                    outline
            ></v-text-field>
        </v-flex>
        <v-flex xs3>
            <v-text-field
                    class="border-1"
                    :label="lastNameLabel"
                    outline
            ></v-text-field>
        </v-flex>
        <v-flex xs3>
            <v-text-field
                    class="border-1"
                    :label="priceLabel"
                    value="50"
                    outline
                    prefix="₪"
                    readonly
            ></v-text-field>
        </v-flex>
    </v-layout>
    <v-layout>
        <v-flex></v-flex>
    </v-layout>
    <v-layout>
        <v-flex></v-flex>
    </v-layout>
    </v-card>
</template>

<script>
    import { LanguageService } from "../../../../services/language/languageService";

    export default {
        name: "userInfoEdit",
        data() {
            return {
                firstNameLabel: LanguageService.getValueByKey("profile_firstName_label"),
                lastNameLabel: LanguageService.getValueByKey("profile_lastName_label"),
                priceLabel: LanguageService.getValueByKey("profile_price_label")
            }
        },
    }
</script>

<style lang="less">
    .user-edit-wrap{
        .v-text-field--outline>.v-input__control>.v-input__slot{
            border: 1px solid rgba(0,0,0,.54);
            &:hover{
                border: 1px solid rgba(0,0,0,.54)!important;
            }
        }
        /*.border-1{*/
            /*.v-input__slot{*/
                /*border: 1px;*/
            /*}*/

        /*}*/
    }


</style>