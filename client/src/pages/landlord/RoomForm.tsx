import React, { useState, useEffect } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { createRoom, updateRoom, uploadRoomImages, type Room } from '../../services/landlordApi';
import api from '../../services/api';
import {
    ArrowLeft,
    Home,
    MapPin,
    DollarSign,
    Users,
    Building2,
    Wifi,
    Truck,
    Sun,
    Flame,
    X,
    Image,
    Loader2
} from 'lucide-react';

export const RoomForm: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const isEditing = !!id;

    const [loading, setLoading] = useState(isEditing);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState('');
    const [files, setFiles] = useState<File[]>([]);
    const [previewUrls, setPreviewUrls] = useState<string[]>([]);

    const [formData, setFormData] = useState({
        title: '',
        description: '',
        address: '',
        city: '',
        state: '',
        zipCode: '',
        latitude: 0,
        longitude: 0,
        price: 0,
        roomType: 'Entire Apartment',
        maxGuests: 1,
        bedrooms: 1,
        bathrooms: 1,
        wiFi: false,
        parking: false,
        ac: false,
        heating: false,
        isAvailable: true
    });

    useEffect(() => {
        if (isEditing) {
            const fetchRoom = async () => {
                try {
                    const response = await api.get(`/rooms/${id}`);
                    const room: Room = response.data.data;
                    setFormData({
                        title: room.title,
                        description: room.description,
                        address: room.address,
                        city: room.city,
                        state: room.state,
                        zipCode: room.zipCode,
                        latitude: room.latitude,
                        longitude: room.longitude,
                        price: room.price,
                        roomType: room.roomType,
                        maxGuests: room.maxGuests,
                        bedrooms: room.bedrooms,
                        bathrooms: room.bathrooms,
                        wiFi: room.wiFi,
                        parking: room.parking,
                        ac: room.ac,
                        heating: room.heating,
                        isAvailable: room.isAvailable
                    });
                } catch (err: any) {
                    setError('Failed to load room data');
                } finally {
                    setLoading(false);
                }
            };
            fetchRoom();
        }
    }, [id, isEditing]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        const { name, value, type } = e.target;
        if (type === 'checkbox') {
            const checked = (e.target as HTMLInputElement).checked;
            setFormData(prev => ({ ...prev, [name]: checked }));
        } else {
            setFormData(prev => ({ ...prev, [name]: type === 'number' ? Number(value) : value }));
        }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files.length > 0) {
            const newFiles = Array.from(e.target.files);
            setFiles(prev => [...prev, ...newFiles]);

            const newUrls = newFiles.map(file => URL.createObjectURL(file));
            setPreviewUrls(prev => [...prev, ...newUrls]);
        }
    };

    const removeFile = (index: number) => {
        setFiles(prev => prev.filter((_, i) => i !== index));
        URL.revokeObjectURL(previewUrls[index]);
        setPreviewUrls(prev => prev.filter((_, i) => i !== index));
    };

    useEffect(() => {
        return () => {
            previewUrls.forEach(url => URL.revokeObjectURL(url));
        };
    }, [previewUrls]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSubmitting(true);
        setError('');

        try {
            let roomId = id;
            if (isEditing) {
                await updateRoom(roomId!, formData);
            } else {
                const response = await createRoom(formData);
                roomId = response.id;
            }

            if (files.length > 0 && roomId) {
                const dataTransfer = new DataTransfer();
                files.forEach(file => dataTransfer.items.add(file));
                await uploadRoomImages(roomId, dataTransfer.files);
            }

            navigate('/landlord/rooms');
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to save room');
        } finally {
            setSubmitting(false);
        }
    };

    if (loading) return (
        <div style={{
            minHeight: '100vh',
            backgroundColor: '#f9fafb',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center'
        }}>
            <div style={{
                animation: 'spin 1s linear infinite',
                borderRadius: '9999px',
                height: '3rem',
                width: '3rem',
                borderBottom: '2px solid #0d9488',
                borderColor: '#e5e7eb',
                borderBottomColor: '#0d9488'
            }}></div>
        </div>
    );

    const inputStyle = {
        width: '100%',
        padding: '0.5rem 1rem',
        border: '1px solid #d1d5db',
        borderRadius: '0.5rem',
        fontSize: '0.875rem',
        color: '#111827',
        backgroundColor: '#ffffff',
        transition: 'all 0.2s',
        outline: 'none'
    };

    const labelStyle = {
        display: 'block',
        fontSize: '0.875rem',
        fontWeight: '500',
        color: '#374151',
        marginBottom: '0.5rem'
    };

    return (
        <div style={{
            minHeight: '100vh',
            backgroundColor: '#f9fafb',
            padding: '2rem 0'
        }}>
            <style>
                {`
                    @keyframes spin {
                        from { transform: rotate(0deg); }
                        to { transform: rotate(360deg); }
                    }
                    input:focus, textarea:focus, select:focus {
                        border-color: #0d9488;
                        box-shadow: 0 0 0 2px rgba(13, 148, 136, 0.2);
                    }
                `}
            </style>
            <div style={{
                maxWidth: '64rem',
                margin: '0 auto',
                padding: '0 1rem'
            }}>
                {/* Header */}
                <div style={{
                    marginBottom: '2rem',
                    display: 'flex',
                    alignItems: 'center',
                    gap: '1rem'
                }}>
                    <Link
                        to="/landlord/rooms"
                        style={{
                            padding: '0.5rem',
                            borderRadius: '0.5rem',
                            color: '#4b5563',
                            textDecoration: 'none',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            transition: 'background-color 0.2s'
                        }}
                        onMouseEnter={(e) => (e.currentTarget.style.backgroundColor = '#f3f4f6')}
                        onMouseLeave={(e) => (e.currentTarget.style.backgroundColor = 'transparent')}
                    >
                        <ArrowLeft size={24} />
                    </Link>
                    <div>
                        <h1 style={{
                            fontSize: '1.875rem',
                            fontWeight: '700',
                            color: '#111827',
                            margin: 0
                        }}>
                            {isEditing ? 'Edit Room' : 'List a New Room'}
                        </h1>
                        <p style={{
                            marginTop: '0.25rem',
                            fontSize: '0.875rem',
                            color: '#6b7280'
                        }}>
                            {isEditing
                                ? 'Update your room details and photos'
                                : 'Fill in the details to start hosting'}
                        </p>
                    </div>
                </div>

                {/* Error Alert */}
                {error && (
                    <div style={{
                        marginBottom: '1.5rem',
                        backgroundColor: '#fef2f2',
                        borderLeft: '4px solid #ef4444',
                        padding: '1rem',
                        borderRadius: '0.5rem'
                    }}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                            <X size={20} color="#ef4444" />
                            <p style={{ fontSize: '0.875rem', color: '#b91c1c', margin: 0 }}>{error}</p>
                        </div>
                    </div>
                )}

                <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '2rem' }}>
                    {/* Basic Information */}
                    <div style={{
                        backgroundColor: '#ffffff',
                        borderRadius: '0.75rem',
                        border: '1px solid #e5e7eb',
                        overflow: 'hidden'
                    }}>
                        <div style={{
                            padding: '1.25rem 1.5rem',
                            borderBottom: '1px solid #e5e7eb',
                            backgroundColor: '#f9fafb'
                        }}>
                            <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                                <Home size={20} color="#0d9488" />
                                <h2 style={{ fontSize: '1.125rem', fontWeight: '600', color: '#111827', margin: 0 }}>Basic Information</h2>
                            </div>
                        </div>
                        <div style={{ padding: '1.5rem', display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
                            <div>
                                <label htmlFor="title" style={labelStyle}>
                                    Title <span style={{ color: '#ef4444' }}>*</span>
                                </label>
                                <input
                                    type="text"
                                    id="title"
                                    name="title"
                                    value={formData.title}
                                    onChange={handleChange}
                                    required
                                    style={inputStyle}
                                    placeholder="E.g., Cozy City Center Apartment"
                                />
                            </div>

                            <div>
                                <label htmlFor="description" style={labelStyle}>
                                    Description <span style={{ color: '#ef4444' }}>*</span>
                                </label>
                                <textarea
                                    id="description"
                                    name="description"
                                    value={formData.description}
                                    onChange={handleChange}
                                    required
                                    rows={5}
                                    style={{ ...inputStyle, resize: 'vertical' }}
                                    placeholder="Describe your place, the neighborhood, and anything else guests should know..."
                                />
                            </div>
                        </div>
                    </div>

                    {/* Location */}
                    <div style={{
                        backgroundColor: '#ffffff',
                        borderRadius: '0.75rem',
                        border: '1px solid #e5e7eb',
                        overflow: 'hidden'
                    }}>
                        <div style={{
                            padding: '1.25rem 1.5rem',
                            borderBottom: '1px solid #e5e7eb',
                            backgroundColor: '#f9fafb'
                        }}>
                            <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                                <MapPin size={20} color="#0d9488" />
                                <h2 style={{ fontSize: '1.125rem', fontWeight: '600', color: '#111827', margin: 0 }}>Location</h2>
                            </div>
                        </div>
                        <div style={{ padding: '1.5rem', display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
                            <div>
                                <label htmlFor="address" style={labelStyle}>
                                    Address <span style={{ color: '#ef4444' }}>*</span>
                                </label>
                                <input
                                    type="text"
                                    id="address"
                                    name="address"
                                    value={formData.address}
                                    onChange={handleChange}
                                    required
                                    style={inputStyle}
                                    placeholder="Street address"
                                />
                            </div>

                            <div style={{
                                display: 'grid',
                                gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
                                gap: '1rem'
                            }}>
                                <div>
                                    <label htmlFor="city" style={labelStyle}>
                                        City <span style={{ color: '#ef4444' }}>*</span>
                                    </label>
                                    <input
                                        type="text"
                                        id="city"
                                        name="city"
                                        value={formData.city}
                                        onChange={handleChange}
                                        required
                                        style={inputStyle}
                                    />
                                </div>
                                <div>
                                    <label htmlFor="state" style={labelStyle}>
                                        State <span style={{ color: '#ef4444' }}>*</span>
                                    </label>
                                    <input
                                        type="text"
                                        id="state"
                                        name="state"
                                        value={formData.state}
                                        onChange={handleChange}
                                        required
                                        style={inputStyle}
                                    />
                                </div>
                                <div>
                                    <label htmlFor="zipCode" style={labelStyle}>
                                        Zip Code <span style={{ color: '#ef4444' }}>*</span>
                                    </label>
                                    <input
                                        type="text"
                                        id="zipCode"
                                        name="zipCode"
                                        value={formData.zipCode}
                                        onChange={handleChange}
                                        required
                                        style={inputStyle}
                                    />
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Listing Details */}
                    <div style={{
                        backgroundColor: '#ffffff',
                        borderRadius: '0.75rem',
                        border: '1px solid #e5e7eb',
                        overflow: 'hidden'
                    }}>
                        <div style={{
                            padding: '1.25rem 1.5rem',
                            borderBottom: '1px solid #e5e7eb',
                            backgroundColor: '#f9fafb'
                        }}>
                            <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                                <Building2 size={20} color="#0d9488" />
                                <h2 style={{ fontSize: '1.125rem', fontWeight: '600', color: '#111827', margin: 0 }}>Listing Details</h2>
                            </div>
                        </div>
                        <div style={{ padding: '1.5rem' }}>
                            <div style={{
                                display: 'grid',
                                gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
                                gap: '1.5rem'
                            }}>
                                <div>
                                    <label htmlFor="price" style={labelStyle}>
                                        Price per night ($) <span style={{ color: '#ef4444' }}>*</span>
                                    </label>
                                    <div style={{ position: 'relative' }}>
                                        <div style={{
                                            position: 'absolute',
                                            left: '0.75rem',
                                            top: '50%',
                                            transform: 'translateY(-50%)',
                                            pointerEvents: 'none'
                                        }}>
                                            <DollarSign size={16} color="#9ca3af" />
                                        </div>
                                        <input
                                            type="number"
                                            id="price"
                                            name="price"
                                            value={formData.price}
                                            onChange={handleChange}
                                            required
                                            min="0"
                                            style={{ ...inputStyle, paddingLeft: '2.5rem' }}
                                            placeholder="0"
                                        />
                                    </div>
                                </div>

                                <div>
                                    <label htmlFor="roomType" style={labelStyle}>
                                        Room Type
                                    </label>
                                    <select
                                        id="roomType"
                                        name="roomType"
                                        value={formData.roomType}
                                        onChange={handleChange}
                                        style={inputStyle}
                                    >
                                        <option value="Entire Apartment">Entire Apartment</option>
                                        <option value="Private Room">Private Room</option>
                                        <option value="Shared Room">Shared Room</option>
                                        <option value="House">House</option>
                                    </select>
                                </div>
                            </div>

                            <div style={{
                                display: 'grid',
                                gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))',
                                gap: '1rem',
                                marginTop: '1.5rem'
                            }}>
                                <div>
                                    <label htmlFor="maxGuests" style={labelStyle}>
                                        Max Guests
                                    </label>
                                    <div style={{ position: 'relative' }}>
                                        <div style={{
                                            position: 'absolute',
                                            left: '0.75rem',
                                            top: '50%',
                                            transform: 'translateY(-50%)',
                                            pointerEvents: 'none'
                                        }}>
                                            <Users size={16} color="#9ca3af" />
                                        </div>
                                        <input
                                            type="number"
                                            id="maxGuests"
                                            name="maxGuests"
                                            value={formData.maxGuests}
                                            onChange={handleChange}
                                            min="1"
                                            style={{ ...inputStyle, paddingLeft: '2.5rem' }}
                                        />
                                    </div>
                                </div>

                                <div>
                                    <label htmlFor="bedrooms" style={labelStyle}>
                                        Bedrooms
                                    </label>
                                    <input
                                        type="number"
                                        id="bedrooms"
                                        name="bedrooms"
                                        value={formData.bedrooms}
                                        onChange={handleChange}
                                        min="0"
                                        style={inputStyle}
                                    />
                                </div>

                                <div>
                                    <label htmlFor="bathrooms" style={labelStyle}>
                                        Bathrooms
                                    </label>
                                    <input
                                        type="number"
                                        id="bathrooms"
                                        name="bathrooms"
                                        value={formData.bathrooms}
                                        onChange={handleChange}
                                        min="0"
                                        step="0.5"
                                        style={inputStyle}
                                    />
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Amenities */}
                    <div style={{
                        backgroundColor: '#ffffff',
                        borderRadius: '0.75rem',
                        border: '1px solid #e5e7eb',
                        overflow: 'hidden'
                    }}>
                        <div style={{
                            padding: '1.25rem 1.5rem',
                            borderBottom: '1px solid #e5e7eb',
                            backgroundColor: '#f9fafb'
                        }}>
                            <h2 style={{ fontSize: '1.125rem', fontWeight: '600', color: '#111827', margin: 0 }}>Amenities</h2>
                        </div>
                        <div style={{ padding: '1.5rem' }}>
                            <div style={{
                                display: 'grid',
                                gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))',
                                gap: '1rem'
                            }}>
                                {[
                                    { name: 'wiFi', icon: Wifi, label: 'WiFi' },
                                    { name: 'parking', icon: Truck, label: 'Parking' },
                                    { name: 'ac', icon: Sun, label: 'Air Conditioning' },
                                    { name: 'heating', icon: Flame, label: 'Heating' }
                                ].map((amenity) => {
                                    const Icon = amenity.icon;
                                    const isChecked = formData[amenity.name as keyof typeof formData] as boolean;
                                    return (
                                        <label
                                            key={amenity.name}
                                            style={{
                                                position: 'relative',
                                                display: 'flex',
                                                alignItems: 'center',
                                                padding: '1rem',
                                                border: `1px solid ${isChecked ? '#0d9488' : '#e5e7eb'}`,
                                                borderRadius: '0.5rem',
                                                cursor: 'pointer',
                                                backgroundColor: isChecked ? '#f0fdfa' : '#ffffff',
                                                transition: 'all 0.2s'
                                            }}
                                            onMouseEnter={(e) => {
                                                if (!isChecked) {
                                                    e.currentTarget.style.backgroundColor = '#f9fafb';
                                                }
                                            }}
                                            onMouseLeave={(e) => {
                                                if (!isChecked) {
                                                    e.currentTarget.style.backgroundColor = '#ffffff';
                                                }
                                            }}
                                        >
                                            <input
                                                type="checkbox"
                                                name={amenity.name}
                                                checked={isChecked}
                                                onChange={handleChange}
                                                style={{ display: 'none' }}
                                            />
                                            <Icon size={20} color={isChecked ? '#0d9488' : '#9ca3af'} />
                                            <span style={{
                                                marginLeft: '0.75rem',
                                                fontSize: '0.875rem',
                                                fontWeight: '500',
                                                color: isChecked ? '#0d9488' : '#374151'
                                            }}>
                                                {amenity.label}
                                            </span>
                                        </label>
                                    );
                                })}
                            </div>
                        </div>
                    </div>

                    {/* Photos */}
                    <div style={{
                        backgroundColor: '#ffffff',
                        borderRadius: '0.75rem',
                        border: '1px solid #e5e7eb',
                        overflow: 'hidden'
                    }}>
                        <div style={{
                            padding: '1.25rem 1.5rem',
                            borderBottom: '1px solid #e5e7eb',
                            backgroundColor: '#f9fafb'
                        }}>
                            <h2 style={{ fontSize: '1.125rem', fontWeight: '600', color: '#111827', margin: 0 }}>Photos</h2>
                        </div>
                        <div style={{ padding: '1.5rem' }}>
                            <div
                                onClick={() => document.getElementById('imageUpload')?.click()}
                                style={{
                                    border: '2px dashed #d1d5db',
                                    borderRadius: '0.5rem',
                                    padding: '2rem',
                                    textAlign: 'center',
                                    cursor: 'pointer',
                                    transition: 'all 0.2s'
                                }}
                                onMouseEnter={(e) => {
                                    e.currentTarget.style.borderColor = '#0d9488';
                                    e.currentTarget.style.backgroundColor = '#f9fafb';
                                }}
                                onMouseLeave={(e) => {
                                    e.currentTarget.style.borderColor = '#d1d5db';
                                    e.currentTarget.style.backgroundColor = 'transparent';
                                }}
                            >
                                <Image size={48} color="#9ca3af" style={{ margin: '0 auto' }} />
                                <div style={{ marginTop: '1rem' }}>
                                    <p style={{ fontSize: '0.875rem', fontWeight: '500', color: '#111827', margin: 0 }}>
                                        Drag and drop your photos here, or click to select
                                    </p>
                                    <p style={{ marginTop: '0.25rem', fontSize: '0.75rem', color: '#9ca3af' }}>
                                        JPG, PNG, GIF up to 5MB each
                                    </p>
                                </div>
                                <input
                                    id="imageUpload"
                                    type="file"
                                    multiple
                                    accept="image/*"
                                    onChange={handleFileChange}
                                    style={{ display: 'none' }}
                                />
                            </div>

                            {previewUrls.length > 0 && (
                                <div style={{
                                    marginTop: '1.5rem',
                                    display: 'grid',
                                    gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))',
                                    gap: '1rem'
                                }}>
                                    {previewUrls.map((url, index) => (
                                        <div
                                            key={url}
                                            style={{
                                                position: 'relative',
                                                aspectRatio: '1',
                                                borderRadius: '0.5rem',
                                                overflow: 'hidden'
                                            }}
                                        >
                                            <img
                                                src={url}
                                                alt={`Preview ${index + 1}`}
                                                style={{
                                                    width: '100%',
                                                    height: '100%',
                                                    objectFit: 'cover'
                                                }}
                                            />
                                            <button
                                                type="button"
                                                onClick={() => removeFile(index)}
                                                style={{
                                                    position: 'absolute',
                                                    top: '0.5rem',
                                                    right: '0.5rem',
                                                    padding: '0.25rem',
                                                    backgroundColor: '#ef4444',
                                                    color: '#ffffff',
                                                    border: 'none',
                                                    borderRadius: '9999px',
                                                    cursor: 'pointer',
                                                    opacity: 0,
                                                    transition: 'opacity 0.2s',
                                                    display: 'flex',
                                                    alignItems: 'center',
                                                    justifyContent: 'center'
                                                }}
                                                onMouseEnter={(e) => {
                                                    e.currentTarget.style.backgroundColor = '#dc2626';
                                                }}
                                                onMouseLeave={(e) => {
                                                    e.currentTarget.style.backgroundColor = '#ef4444';
                                                }}
                                            >
                                                <X size={16} />
                                            </button>
                                            <style>
                                                {`
                                                    div:hover button {
                                                        opacity: 1;
                                                    }
                                                `}
                                            </style>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Form Actions */}
                    <div style={{
                        display: 'flex',
                        justifyContent: 'flex-end',
                        gap: '1rem',
                        paddingTop: '1rem'
                    }}>
                        <Link
                            to="/landlord/rooms"
                            style={{
                                padding: '0.5rem 1.5rem',
                                border: '1px solid #d1d5db',
                                borderRadius: '0.5rem',
                                fontSize: '0.875rem',
                                fontWeight: '500',
                                color: '#374151',
                                backgroundColor: '#ffffff',
                                textDecoration: 'none',
                                transition: 'all 0.2s'
                            }}
                            onMouseEnter={(e) => {
                                e.currentTarget.style.backgroundColor = '#f9fafb';
                            }}
                            onMouseLeave={(e) => {
                                e.currentTarget.style.backgroundColor = '#ffffff';
                            }}
                        >
                            Cancel
                        </Link>
                        <button
                            type="submit"
                            disabled={submitting}
                            style={{
                                padding: '0.5rem 1.5rem',
                                border: 'none',
                                borderRadius: '0.5rem',
                                fontSize: '0.875rem',
                                fontWeight: '500',
                                color: '#ffffff',
                                backgroundColor: submitting ? '#99a1af' : '#0d9488',
                                cursor: submitting ? 'not-allowed' : 'pointer',
                                transition: 'all 0.2s',
                                display: 'flex',
                                alignItems: 'center',
                                gap: '0.5rem'
                            }}
                            onMouseEnter={(e) => {
                                if (!submitting) {
                                    e.currentTarget.style.backgroundColor = '#0f766e';
                                }
                            }}
                            onMouseLeave={(e) => {
                                if (!submitting) {
                                    e.currentTarget.style.backgroundColor = '#0d9488';
                                }
                            }}
                        >
                            {submitting ? (
                                <>
                                    <Loader2 size={16} style={{ animation: 'spin 1s linear infinite' }} />
                                    Saving...
                                </>
                            ) : (
                                'Save Room'
                            )}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};