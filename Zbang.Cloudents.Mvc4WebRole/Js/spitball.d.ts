declare namespace spitaball {
    //export type cacheKeys = "university" | "accountDetail" | "html" | "department" | "itemComment";

   // export type hubEvent = "hub-chat" | "hub-status" | "preview-ready" | "update-thumbnail" | "connection-state"

    interface ISpitballStateParamsService extends angular.ui.IStateParamsService {
        boxId: number;
        itemId: number;
        userId: number;
    }
}

interface String {
    startsWith(str: string): boolean;
    endsWith(str: string): boolean;
}
interface IAnalytics extends angular.google.analytics.AnalyticsService {
    trackTimings(timingCategory: string, timingVar: string, timingValue: number, timingLabel: string);
}
interface Array<T> {
    findIndex(predicate: (search: T) => boolean): number;
    find(predicate: (search: T) => boolean): T;
}