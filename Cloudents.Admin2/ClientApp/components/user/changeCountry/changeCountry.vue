<template>
    <v-container class="changeCountry-cont">
        <h2 class="mb-3">Change Country</h2>
        <v-form @submit.prevent="changeCountry">
            <div class="changeCountry">
                    <v-text-field
                        v-model="country"
                        label="Country"
                        single-line
                        solo
                        prepend-inner-icon="place"
                    ></v-text-field>
                    <v-text-field
                        v-model="id"
                        label="Id"
                        single-line
                        solo
                    ></v-text-field>

                <v-flex>
                    <v-btn color="primary" class="white--text" type="submit">Send</v-btn>
                </v-flex>
            </div>
        </v-form>
    </v-container>
</template>

<script>
import changeCountryService from './changeCountry';

export default {
    name: 'changeCountry',
    data() {
        return {
            country: '',
            id: ''
        }
    },
    methods: {
        changeCountry() {
            let userObj = {id: this.id, country: this.country.toUpperCase()}
            changeCountryService.updateCountry(userObj).then(res => {
                this.$toaster.success(`Success: change user ${this.id} country`);
            }, (err) => {
                this.$toaster.error(`Error: change user ${this.id} country`);
            }).finally(() => {
                this.country = '';
                this.id = '';
            })
        }
    }
}
</script>

<style lang="less">
.changeCountry-cont{
    text-align: center;
    .changeCountry{
        max-width: 400px;
        margin: 0 auto;
    }
}
</style>