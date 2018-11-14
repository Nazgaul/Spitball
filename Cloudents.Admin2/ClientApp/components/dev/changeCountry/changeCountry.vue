<template>
    <div>
        <h1>Change User Country</h1>
        User Id:<input type="text" v-model="userId"/>
        <select class="select-type" v-model="userCountry">
            <option value="Us">US</option>
            <option value="Il">IL</option>
        </select>
    </div>
</template>

<script>
import { setUserCountry } from './changeCountryService.js'

export default {
    userCountry: "Us",
    userId: ""

}
</script>

<style lang="scss" scoped>
</style>
