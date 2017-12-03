<template functional>
    <v-expansion-panel expand>
        <v-expansion-panel-content :value=true>
            <div slot="header">{{props.title}}</div>
            <div class="sort-filter">
                <v-checkbox v-if="props.values" v-for="s in props.values" @click="props.callback(s)" :label="s" :key="s" hide-details></v-checkbox>
            </div>
        </v-expansion-panel-content>
    </v-expansion-panel>
</template>
            changeCallback: { type: Function },
            title:{}
<style src="./radioList.less" lang="less"></style>
