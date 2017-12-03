<template functional>
    <v-expansion-panel expand>
        <v-expansion-panel-content v-for="k in props.values" :key="k.modelId">
            <div slot="header">{{k.title}}</div>
            <div class="sort-filter">
                <v-checkbox v-if="k.data" v-for="s in k.data" :inputValue="props.checkesVals.includes(s.id?s.id.toString():s)" @change="props.callback({id:k.modelId,val:(s.id?s.id:s),type:$event})" :label="(s.name?s.name:s)" :key="(s.id?s.id:s)" hide-details></v-checkbox>
            </div>
        </v-expansion-panel-content>
    </v-expansion-panel>
</template>
<style src="./radioList.less" lang="less"></style>
