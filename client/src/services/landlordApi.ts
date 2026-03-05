import api from './api';

export interface DashboardStats {
    totalRooms: number;
    activeRooms: number;
    totalBookings: number;
    pendingBookings: number;
}

export interface Room {
    id: string;
    landlordId: number;
    title: string;
    description: string;
    address: string;
    city: string;
    state: string;
    zipCode: string;
    latitude: number;
    longitude: number;
    price: number;
    roomType: string;
    maxGuests: number;
    bedrooms: number;
    bathrooms: number;
    wiFi: boolean;
    parking: boolean;
    ac: boolean;
    heating: boolean;
    imageUrl: string;
    isAvailable: boolean;
}

export interface Booking {
    id: number;
    userId: number;
    roomId: string;
    checkInDate: string;
    checkOutDate: string;
    numberOfGuests: number;
    totalPrice: number;
    status: string;
    notes?: string;
}

export const getDashboardStats = async (): Promise<DashboardStats> => {
    const response = await api.get('/landlord/dashboard');
    return response.data.data;
};

export const getLandlordRooms = async (landlordId: number): Promise<Room[]> => {
    const response = await api.get(`/rooms/landlord/${landlordId}`);
    return response.data.data;
};

export const createRoom = async (roomData: any): Promise<any> => {
    const response = await api.post('/rooms', roomData);
    return response.data.data;
};

export const updateRoom = async (roomId: string, roomData: any): Promise<void> => {
    await api.put(`/rooms/${roomId}`, roomData);
};

export const deleteRoom = async (roomId: string): Promise<void> => {
    await api.delete(`/rooms/${roomId}`);
};

export const uploadRoomImages = async (roomId: string, files: FileList): Promise<string[]> => {
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
        formData.append('images', files[i]);
    }
    const response = await api.post(`/landlord/rooms/${roomId}/images`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    });
    return response.data.data;
};

export const getLandlordBookings = async (): Promise<Booking[]> => {
    const response = await api.get('/landlord/bookings');
    return response.data.data;
};

export const approveBooking = async (bookingId: number): Promise<void> => {
    await api.patch(`/landlord/bookings/${bookingId}/approve`);
};

export const rejectBooking = async (bookingId: number): Promise<void> => {
    await api.patch(`/landlord/bookings/${bookingId}/reject`);
};
