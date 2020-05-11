<template functional>
    <div class="tutorPrice">
        <i18n-n :value="props.discount || props.price" tag="div" :locale="'he-IL'" :format="{ key: 'currency', /* currency: props.currency */ }">
            <template v-slot:integer="slotProps">
                <span class="number">{{ slotProps.integer }}</span>
            </template>
            <template v-slot:currency="slotProps">
                <span class="currency d-inline-block"> {{ slotProps.currency }}</span>/
                <span class="hour" v-t="'profile_points_hour'"></span>
            </template>
        </i18n-n>
    </div>
</template>