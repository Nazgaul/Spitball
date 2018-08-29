<template>
     <v-dialog ref="person" v-model="showDialog" v-if="value" max-width="520" :fullscreen="isSearch" :persistent="!isSearch" :content-class="isSearch?'dialog-choose':'ma-3'" :hide-overlay="isSearch">
        <v-card class="personalize-wrapper" v-show="!isSearch">
            <v-card-title class="" v-language:inner>resultPersonalize_personalize_results</v-card-title>
            <v-card-text class="pb-2">
                <v-text-field class="elevation-0" type="search" auto-grow solo prepend-icon="sbf-search" placeholder="Where do you go to school?" @click="$_personalize"></v-text-field>
            </v-card-text>
            <v-card-actions class="pb-1">
                <v-spacer></v-spacer>
                <v-btn flat="flat" @click.native="showDialog = false" v-language:inner>resultPersonalize_not_now</v-btn>
                <v-btn flat="flat" @click.native="$_personalize" v-language:inner>resultPersonalize_personalize</v-btn>
            </v-card-actions>
        </v-card>
         <search-item ref="search" v-model="showDialog" v-show="isSearch" :type="type" :keep="keep" :isFirst="isfirst" :isShown="isSearch&&showDialog"></search-item>
    </v-dialog>
</template>
<style lang="less" src="./ResultPersonalize.less">
</style>
<script src="./ResultPersonalize.js">
</script>